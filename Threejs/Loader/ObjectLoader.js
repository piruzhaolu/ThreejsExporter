import * as THREE from "../build/three.module.js";
import {BundleLoader} from "./BundleLoader.js";

export class ObjectLoader extends THREE.FileLoader {

    _geometrieMap;
    _materialMap;
    _objectMap;
    _asyncLoadArray;
    _parent;

    constructor() {
        super();
        this._geometrieMap = new Map();
        this._materialMap = new Map();
        this._objectMap = new Map();
        this._asyncLoadArray = [];
    }

    setParent(parent){
        this._parent = parent;
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
                        if (Array.isArray(geo.groups) && geo.groups.length>0){
                            for (let i = 0; i < geo.groups.length; i++){
                                let sub = geo.groups[i];
                                geometry.addGroup(sub.start, sub.count, sub.materialIndex);
                            }
                        }

                        mThis._geometrieMap.set(geo.id, geometry);
                        mThis._asyncLoadArray.push({
                            target: geometry,
                            path: geo.attr_position,
                            callback: function (target, arrayBuffer) {
                                let vertices = new Float32Array(arrayBuffer);
                                target.setAttribute('position', new THREE.BufferAttribute(vertices, 3));
                                target.computeBoundingSphere();
                            }
                        });
                        mThis._asyncLoadArray.push({
                            target: geometry,
                            path: geo.indexs,
                            callback: function (target, arrayBuffer) {
                                target.setIndex([...new Int32Array(arrayBuffer)]);
                                target.computeBoundingSphere();
                            }
                        });
                        mThis._asyncLoadArray.push({
                            target:geometry,
                            path:geo.attr_normal,
                            callback:function (target, arrayBuffer) {
                                let normal = new Float32Array(arrayBuffer);
                                target.setAttribute('normal', new THREE.BufferAttribute(normal, 3));
                            }
                        });
                        mThis._asyncLoadArray.push({
                            target:geometry,
                            path:geo.attr_uv,
                            callback:function (target, arrayBuffer) {
                                let uv = new Float32Array(arrayBuffer);
                                target.setAttribute('uv', new THREE.BufferAttribute(uv, 2));
                            }
                        });
                    }

                    let materials = objAsJson.materials;
                    for (let mat of materials){

                        let m = new THREE.MeshStandardMaterial();
                        if (Array.isArray(mat.color)){
                            m.color = new THREE.Color(mat.color[0],mat.color[1],mat.color[2]);
                        }
                        if (typeof mat.map == 'string' && mat.map.length>0){
                            m.map = new THREE.TextureLoader().load("../assets/" + mat.map);
                        }
                        // m.emissive = new THREE.Color(0.2,0.2,0.2);
                        // m.emissiveIntensity = 0.2;
                        // m.metalness = 0.5;
                        mThis._materialMap.set(mat.id, m);
                    }



                    let objects = objAsJson.objects;
                    for (let obj of objects) {
                        if (obj.geometry == "") continue;
                        let geometry = mThis._geometrieMap.get(obj.geometry);
                        let material;
                        if (Array.isArray(obj.materials) && obj.materials.length>0){
                            material = [];
                            for (let i = 0; i < obj.materials.length;i++){
                                material[i] = mThis._materialMap.get(obj.materials[i]);
                            }
                            // for (let matElement of obj.materials) {
                            //     material.push(mThis._materialMap.get(matElement));
                            // }
                            //material = material[2];
                        }else{
                            material = mThis._materialMap.get(obj.material);// new THREE.MeshLambertMaterial({color: 0xff9999});
                        }
                        let mesh = new THREE.Mesh(geometry, material);
                        // mesh.applyMatrix4(new THREE.Matrix4().set([...obj.matrix]));
                        let parentObj = mThis._objectMap.get(obj.parent);
                        if (parentObj != undefined){
                            //mesh.parent = parentObj;
                            parentObj.add(mesh);
                        }else if (mThis._parent != undefined){
                            mThis._parent.add(mesh);
                        }
                        mesh.position.set(...obj.position);
                        mesh.quaternion.set(...obj.quaternion);
                        mThis._objectMap.set(obj.id, mesh);
                    }

                }
                onLoad();
                mThis._asyncLoad();
            },
            function (xhr) {
                console.log((xhr.loaded / xhr.total * 100) + '% loaded');
            },

            function (err) {
                console.error('An error happened');
            }
        )
    }

    _asyncLoad() {
        for (let asyncLoadMapElement of this._asyncLoadArray) {
            //TODO:优化内嵌函数
            (function (element) {
                let loader = new BundleLoader();
                let array = element.path.split("#");
                let index = 0;
                if (array.length > 0) {
                    index = array[1] - 0;
                }

                //TODO:路径需要路由处理
                loader.load("../assets/" + array[0], function (_) {
                    let data = loader.get(index);
                    element.callback(asyncLoadMapElement.target, data);
                });
            })(asyncLoadMapElement);

        }
    }

    addTo(scene) {
        for (let obj of this._objectMap.values()) {
            scene.add(obj);
            // console.log(obj);
        }
    }


}