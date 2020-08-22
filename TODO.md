## TODO
- 阴影范围的表示与unity不同，threejs 使用light.shadow.camera的六个平面内来表示阴影区域。先提供区域设置脚本，后续在threejs里提供模拟脚本 【Done】
- threejs似乎不能设置材质的skybox。 如果skybox为 cubetexture则支持，其它的忽略【Done】
- 需要支持AmbientLight、HemisphereLight，skybox的环境光忽略 【Done】
- play 直接打开浏览器
- 其它材质的支持
- normal 贴图在某些系统不兼容问题
- 自定义数据系统 
- 坐标系转换