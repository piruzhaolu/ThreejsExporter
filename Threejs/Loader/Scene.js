import * as THREE from "../build/three.module.js";
import {BundleLoader} from "./BundleLoader.js";
import {ObjectLoader} from "./ObjectLoader.js";


export class Scene extends THREE.Scene {

    _objectLoader;

    constructor() {
        super();
    }

    load(url, onLoad){
        let loader = new ObjectLoader();
        loader.setParent(this);
        loader.load(url, function (_) {
            onLoad();
        });
        this._objectLoader = loader;
    }


    getCamera(){
        return this._objectLoader.getCamera();
    }



}