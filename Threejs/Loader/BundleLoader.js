import * as THREE from "../build/three.module.js";

export class BundleLoader extends THREE.FileLoader {

    _map;

    constructor() {
        super();
        this.setResponseType("arraybuffer");

    }


    load(url, onLoad, onProgress, onError) {
        let mThis = this;
        super.load(url,
            function (response) {
                let arrayBuffer = response;
                let length = new Int32Array(arrayBuffer, 0, 1)[0];
                let headItemLength = 4 * 4;
                let headView = new DataView(arrayBuffer, 4, length * headItemLength);

                mThis._map = new Map();

                for (let i = 0; i < length; i++) {

                    let id = headView.getInt32(i * headItemLength, true);
                    let dataType = headView.getInt32(i * headItemLength + 4, true);
                    let offset = headView.getInt32(i * headItemLength + 8, true);
                    let byteLength = headView.getInt32(i * headItemLength + 12, true);
                    mThis._map.set(id, arrayBuffer.slice(offset, offset + byteLength));// new Int8Array(arrayBuffer,offset, byteLength);
                }
                onLoad(mThis);
            },
            function (xhr) {
                console.log((xhr.loaded / xhr.total * 100) + '% loaded');
            },

            function (err) {
                console.error('An error happened');
            }
        )
    }

    get(id) {
        // if (id === undefined){
        //     let geometry = new THREE.BufferGeometry();
        //     let vertices = new Float32Array(this._map.get(0));
        //     geometry.setIndex([...new Int32Array(this._map.get(1))]);
        //     geometry.setAttribute('position', new THREE.BufferAttribute(vertices, 3));
        //     let material = new THREE.MeshBasicMaterial({color: 0xff9999});
        //     let mesh = new THREE.Mesh(geometry, material);
        //     return mesh;
        // }
        return this._map.get(id);
    }

}