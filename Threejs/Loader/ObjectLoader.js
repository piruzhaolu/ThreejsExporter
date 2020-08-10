import * as THREE from "../build/three.module.js";
import {BundleLoader} from "./BundleLoader.js";

export class ObjectLoader extends THREE.FileLoader {

    _geometrieMap;
    _materialMap;
    _objectMap;
    //_asyncLoadArray;
    _parent;
    _routing;
    _bundleMap;
    _camera;

    constructor() {
        super();
        this._geometrieMap = new Map();
        this._materialMap = new Map();
        this._objectMap = new Map();
       // this._asyncLoadArray = [];
        this._routing = function (path) {
            return "../assets/" + path;
        }
        this._bundleMap = new Map();
    }

    //当前所加载对象默认添加到的容器
    setParent(parent){
        this._parent = parent;
    }

    setRouting(routing) {
        this._routing = routing;
    }

    getCamera(){
        return this._camera;
    }


    load(url, onLoad, onProgress, onError) {
        let mThis = this;
        super.load(url,
            function (response) {
                let objAsJson = JSON.parse(response);
                if (objAsJson.metadata.type == "object") {
                    let geometries = objAsJson.geometries;
                    for (let geo of geometries) {
                        let geometry = new THREE.BufferGeometry();
                        if (Array.isArray(geo.groups) && geo.groups.length>0){//sub mesh
                            for (let i = 0; i < geo.groups.length; i++){
                                let sub = geo.groups[i];
                                geometry.addGroup(sub.start, sub.count, sub.materialIndex);
                            }
                        }
                        mThis._geometrieMap.set(geo.id, geometry);
                        mThis._setGeometry(geometry,geo);
                    }


                    let materials = objAsJson.materials;
                    for (let mat of materials){
                        let m = mThis._createMaterial(mat);
                        mThis._materialMap.set(mat.id, m);
                    }

                    let objects = objAsJson.objects;
                    for (let obj of objects) {
                        // if (obj.geometry == "" || obj.geometry == undefined) {
                        //
                        //     continue;
                        // }
                        if (obj.type == "mesh") {
                            mThis._createMeshObject(obj);
                        }else if(obj.type == "group") {
                            console.log(obj);
                        }

                        mThis._setDatas(obj.datas, obj);
                    }

                }
                onLoad();
            },
            function (xhr) {
                console.log((xhr.loaded / xhr.total * 100) + '% loaded');
            },

            function (err) {
                console.error('An error happened');
            }
        )
    }



    _setDatas(datas, obj){
        if (Array.isArray(datas)){
            for (let data of datas) {
                if (data.type == "directionalLight"){
                    this._setDirectionalLight(data);
                } else if (data.type == "perspectiveCamera" ){
                    this._setCamera(data,obj);
                }
            }
        }
    }


    _setDirectionalLight(lightData){
        let color = new THREE.Color(...lightData.color);
        let light = new THREE.DirectionalLight( color, lightData.intensity );
        light.position.set( ...lightData.position);
        light.castShadow = lightData.castShadow;

        light.shadow.mapSize.width = lightData.shadowMapSizeW;
        light.shadow.mapSize.height = lightData.shadowMapSizeH;
        light.shadow.camera.near = lightData.shadowNear;    // default
        light.shadow.camera.far = lightData.shadowFar;     // default

        light.shadow.camera.left = lightData.shadowCameraLeft;
        light.shadow.camera.right =  lightData.shadowCameraRight;
        light.shadow.camera.top =  lightData.shadowCameraTop;
        light.shadow.camera.bottom = lightData.shadowCameraBottom;
        if (this._parent != undefined){
            this._parent.add(light);
        }
    }

    _setCamera(cameraData, obj) {
        let camera = new THREE.PerspectiveCamera( cameraData.fov, window.innerWidth / window.innerHeight, cameraData.near, cameraData.far);
        //camera.position.set(...obj.position);
        camera.position.set(obj.position[0],-obj.position[1],obj.position[2]);
        //camera.lookAt( 0, 0, 0 );
        camera.quaternion.set(...obj.quaternion);
       // camera.scale.set(...obj.scale);
        camera.updateProjectionMatrix();
        console.log(camera);
        this._camera = camera;
    }

    //create mesh object
    _createMeshObject(obj){
        let geometry = this._geometrieMap.get(obj.geometry);
        let material;
        if (Array.isArray(obj.materials) && obj.materials.length>0){
            material = [];
            for (let i = 0; i < obj.materials.length;i++){
                material[i] = this._materialMap.get(obj.materials[i]);
            }
        }else{
            material = this._materialMap.get(obj.material);// new THREE.MeshLambertMaterial({color: 0xff9999});
        }
        let mesh = new THREE.Mesh(geometry, material);
        // mesh.applyMatrix4(new THREE.Matrix4().set([...obj.matrix]));
        let parentObj = this._objectMap.get(obj.parent);
        if (parentObj != undefined){
            //mesh.parent = parentObj;
            parentObj.add(mesh);
        }else if (this._parent != undefined){
            this._parent.add(mesh);
        }
        mesh.position.set(...obj.position);
        mesh.quaternion.set(...obj.quaternion);
        mesh.scale.set(...obj.scale);
        mesh.castShadow = true;
        mesh.receiveShadow = true;
        this._objectMap.set(obj.id, mesh);
    }


    _createMaterial(mat){
        let m = new THREE.MeshStandardMaterial();
        if (Array.isArray(mat.color)){
            m.color = new THREE.Color(mat.color[0],mat.color[1],mat.color[2]);
        }
        if (typeof mat.map == 'string' && mat.map != ""){
            m.map = new THREE.TextureLoader().load( this._routing(mat.map));//TODO:不知道内部是否重复加载
        }

        if (typeof mat.metalnessMap == 'string' && mat.metalnessMap != ""){
            m.metalnessMap = m.roughnessMap = new THREE.TextureLoader().load( this._routing(mat.metalnessMap));
        }

        if (typeof mat.normalMap == 'string' && mat.normalMap != ""){
            m.normalMap = new THREE.TextureLoader().load( this._routing(mat.normalMap));
        }

        //m.metalness = 1;
        m.metalness = mat.metalness;

        // m.emissive = new THREE.Color(0.2,0.2,0.2);
        // m.emissiveIntensity = 0.2;
        // m.metalness = 0.5;
        return m;
    }

    _setGeometry(geometry, geo){
        this._loadBundle(geo.attr_position, function (arrayBuffer) {
            let vertices = new Float32Array(arrayBuffer);
            geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));
            geometry.computeBoundingSphere();
        });
        this._loadBundle(geo.indexs, function (arrayBuffer) {
            geometry.setIndex([...new Int32Array(arrayBuffer)]);
            geometry.computeBoundingSphere();
        });
        this._loadBundle(geo.attr_normal, function (arrayBuffer) {
            let normal = new Float32Array(arrayBuffer);
            geometry.setAttribute('normal', new THREE.BufferAttribute(normal, 3));
        });
        this._loadBundle(geo.attr_uv, function (arrayBuffer) {
            let uv = new Float32Array(arrayBuffer);
            geometry.setAttribute('uv', new THREE.BufferAttribute(uv, 2));
        });

    }

    _loadBundle(path, callbackFn){
        let array = path.split("#");
        let path2 = array[0];
        let index = 0;
        if (array.length > 0) {
            index = array[1] - 0;
        }
        let bundleLoader = this._bundleMap[path2];
        if (bundleLoader == undefined){
            bundleLoader = new BundleLoader();
            this._bundleMap[path2] = bundleLoader;
            bundleLoader.load(this._routing(path2)).addListener(callbackFn, index);
        }else{
            bundleLoader.addListener(callbackFn, index);
        }

    }



    // addTo(scene) {
    //     for (let obj of this._objectMap.values()) {
    //         scene.add(obj);
    //         // console.log(obj);
    //     }
    // }


}