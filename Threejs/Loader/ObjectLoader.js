import * as THREE from "../build/three.module.js";
import {BundleLoader} from "./BundleLoader.js";

export class ObjectLoader extends THREE.FileLoader {

    _geometrieMap;
    _objectMap;
    _asyncLoadArray;

    constructor() {
        super();
        this._geometrieMap = new Map();
        this._objectMap = new Map();
        this._asyncLoadArray = [];
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
                        mThis._geometrieMap.set(geo.id, geometry);
                        mThis._asyncLoadArray.push({
                            target:geometry,
                            path:geo.attr_position,
                            callback:function (target, arrayBuffer) {
                                let vertices = new Float32Array(arrayBuffer);
                                target.setAttribute('position', new THREE.BufferAttribute(vertices, 3));
                            }
                        });
                        mThis._asyncLoadArray.push({
                            target:geometry,
                            path:geo.indexs,
                            callback:function (target, arrayBuffer) {
                                target.setIndex([...new Int32Array(arrayBuffer)]);
                            }
                        });
                    }

                    let objects = objAsJson.objects;
                    for (let obj of objects) {
                        let geometry = mThis._geometrieMap.get(obj.geometry);
                        let material = new THREE.MeshBasicMaterial({color: 0xff9999});
                        let mesh = new THREE.Mesh(geometry, material);
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

    _asyncLoad(){
        for (let asyncLoadMapElement of this._asyncLoadArray) {
            let loader = new BundleLoader();
            let array = asyncLoadMapElement.path.split("#");
            let index = 0;
            if (array.length>0){
                index = array[1]-0;
            }

            loader.load(array[0], function (_) {
                let data = loader.get(index);
                asyncLoadMapElement.callback(asyncLoadMapElement.target, data);
            });
        }
    }

    addTo(scene){
        for (let obj of this._objectMap.values()) {
            scene.add(obj);
        }
    }


}