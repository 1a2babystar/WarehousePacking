let Previewscene, Previewcamera
let Previewcontrols, cargo
let material
let raxis = "y"

$(`table#CargoInfo tr:nth-child(2) .CargoRotationInfo`).change(Rerender);

function SetPreviewCargo(rowIndex) {
    PreviewRemove();

    var width = $(`table#CargoInfo tr:nth-child(${rowIndex + 1}) .CargoWidth`).val()
    var height = $(`table#CargoInfo tr:nth-child(${rowIndex + 1}) .CargoHeight`).val()
    var depth = $(`table#CargoInfo tr:nth-child(${rowIndex + 1}) .CargoDepth`).val()

    // var width = $(`#CargoWidth${rowIndex}`).val()
    // var height = $(`#CargoHeight${rowIndex}`).val()
    // var depth = $(`#CargoDepth${rowIndex}`).val()

    const cargogeometry = new THREE.BoxGeometry(height, depth, width);
    cargo = new THREE.Mesh(cargogeometry, material)
    cargo.position.set(0, 0, 0)

    Previewscene.add(cargo)

    Previewcamera.position.set(1.5 * height, 1.5 * depth, 1.5 * width)
}

function Rerender() {
    var rowIndex = $(this).closest('tr').index();
    SetPreviewCargo(rowIndex)
    var axis = $(this).val()
    switch (axis) {
        case "width":
            cargo.rotation.x = Math.PI / 2
            cargo.rotation.y = 0
            cargo.rotation.z = 0
            raxis = "z"
            break
        case "height":
            cargo.rotation.x = 0
            cargo.rotation.y = 0
            cargo.rotation.z = Math.PI / 2
            raxis = "y"
            break
        default:
            cargo.rotation.x = 0
            cargo.rotation.y = 0
            cargo.rotation.z = 0
            raxis = "y"
            break
    }
}

function PreviewInit() {
    Previewcamera = new THREE.PerspectiveCamera(45, $('#PreView').width() / $('#PreView').height(), 0.1, 10000);
    Previewcamera.lookAt(0, 0, 0);

    Previewscene = new THREE.Scene();
    Previewscene.background = new THREE.Color(0xf0f0f0);
    Previewscene.add(camera)

    const textureloader = new THREE.TextureLoader()
    const normaltexture = textureloader.load('/textures/box.jpg')
    material = new THREE.MeshBasicMaterial({ map: normaltexture });

    const axesHelper = new THREE.AxesHelper(100);
    Previewscene.add(axesHelper);

    Previewrenderer = new THREE.WebGLRenderer({ antialias: true });
    Previewrenderer.setClearColor(0xf0f0f0);
    Previewrenderer.setPixelRatio(window.devicePixelRatio);
    Previewrenderer.setSize($('#PreView').width(), $('#PreView').height());

    Previewcontrols = new THREE.OrbitControls(Previewcamera, Previewrenderer.domElement);
    $('#PreView').append(Previewrenderer.domElement);

    window.addEventListener('resize', onWindowPreview, false);

    const cargogeometry = new THREE.BoxGeometry(1, 1, 1);
    cargo = new THREE.Mesh(cargogeometry, material)
    cargo.position.set(0, 0, 0)

    Previewscene.add(cargo)
}

function onWindowPreview() {
    Previewcamera.aspect = $('#PreView').width() / $('#PreView').height();
    Previewcamera.updateProjectionMatrix();

    Previewrenderer.setSize($('#PreView').width(), $('#PreView').height());
}

const clock = new THREE.Clock()

function Previewanimate() {
    const elapsedtime = clock.getElapsedTime()


    if (raxis == "y") {
        cargo.rotation.y = 0.5 * elapsedtime
    }
    else if (raxis == "z") {
        cargo.rotation.z = 0.5 * elapsedtime
    }

    requestAnimationFrame(Previewanimate);

    Previewcontrols.update();

    Previewrenderer.render(Previewscene, Previewcamera);
}

function PreviewRemove() {
    for (var i = Previewscene.children.length - 1; i >= 1; i--) {
        obj = Previewscene.children[i];
        Previewscene.remove(obj);
    }
}

PreviewInit();
Previewanimate();
