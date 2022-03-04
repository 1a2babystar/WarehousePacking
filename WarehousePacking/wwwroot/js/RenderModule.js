let containergeometry, containermaterial
let controls
let origin
let cargomaterial
let Resultlist
let ProcessInfo = {}
let textureloader
let shcontainer = 1
let pallethorizontal, palletvertical, palletmiddle, palletbottom
let light

const Testdata = {
    Container: [
        {
            name: "Container1",
            width: 20,
            height: 20,
            depth: 20,
            maxweight: 100
        },
        {
            name: "Container2",
            width: 30,
            height: 10,
            depth: 10,
            maxweight: 100
        }
    ],
    Cargo: [
        {
            name: "box1",
            width: 5,
            height: 5,
            depth: 5,
            weight: 1,
            count: 10,
            rotation: "all"
        },
        {
            name: "box2",
            width: 3,
            height: 5,
            depth: 7,
            weight: 1,
            count: 10,
            rotation: "all"
        },
        {
            name: "box3",
            width: 2,
            height: 1,
            depth: 2,
            weight: 1,
            count: 10,
            rotation: "all"
        }
    ]
}

init();
animate();
CalculateStart();

$("#hidecontainer").click(function () {
    if (shcontainer) {
        let rowIndex = ProcessInfo['index']
        let obj = Resultlist.find(obj => obj.binindex == rowIndex)
        var bin = obj.bininfo
        obj = scene.children.find(obj => obj.name == "container")
        scene.remove(obj);
        SetPallet(bin.width, bin.height, bin.depth)
        shcontainer = 0
    }
    else {
        let rowIndex = ProcessInfo['index']
        let obj = Resultlist.find(obj => obj.binindex == rowIndex)
        var bin = obj.bininfo
        RemovePallet();
        AddContainer(bin.width, bin.height, bin.depth);
        shcontainer = 1
    }
})

$("#ContainerInfo").on('click', '.resultview', function (e) {
    RemoveElement();
    var rowIndex = $(this).closest('tr').index();
    ProcessInfo['index'] = rowIndex;
    ProcessInfo['binorder'] = 0;
    ProcessInfo['count'] = 0;
    shcontainer = 1;
    var obj = Resultlist.find(obj => obj.binindex == rowIndex)
    var item = obj.information[0];
    document.getElementById('containertype').innerHTML = `Type 1 (x ${item.count})`;
    var bin = obj.bininfo
    AddContainer(bin.width, bin.height, bin.depth);
    camera.position.set(bin.height * 1.5, bin.depth * 1.5, bin.width * 1.5)
    light.position.set(bin.height * 10, bin.depth * 10, bin.width * 10)
});

$("#ContainerInfo").on('click', '.deleterow', function (e) {
    let rowIndex = $(this).closest('tr').index();
    $(`table#ContainerInfo tr:nth-child(${rowIndex + 1})`).remove();
    ResetContainerIndex();
})

function ResetContainerIndex() {
    var count = document.getElementById("ContainerInfo").rows.length;
    for (var cont = 1; cont <= count; cont++) {
        $(`table#ContainerInfo tr:nth-child(${cont + 1}) td:first`).html(`${cont}`)
    }
}

$("#CargoInfo").on('click', '.deleterow', function (e) {
    let rowIndex = $(this).closest('tr').index();
    $(`table#CargoInfo tr:nth-child(${rowIndex + 1})`).remove();
    ResetCargoIndex();
})

function ResetCargoIndex() {
    var count = document.getElementById("CargoInfo").rows.length;
    for (var cont = 1; cont <= count; cont++) {
        $(`table#CargoInfo tr:nth-child(${cont + 1}) td:first`).html(`${cont}`)
    }
}

$("#gobtn").click(function () {
    let rowIndex = ProcessInfo['index'];
    let binorder = ProcessInfo['binorder'];
    let count = ProcessInfo['count']
    var obj = Resultlist.find(obj => obj.binindex == rowIndex);
    var iteminfo = obj.information[binorder].fitlist;
    if (iteminfo.length <= count) return
    var bin = obj.bininfo
    origin = [-bin.width / 2, -bin.height / 2, -bin.depth / 2];
    AddCargo(iteminfo[count], count);
    ProcessInfo['count'] += 1;
})

$("#backbtn").click(function () {
    len = scene.children.length;
    if (len <= 2) return
    obj = scene.children.find(obj => obj.name == `${ProcessInfo['count'] - 1}`)
    scene.remove(obj);
    ProcessInfo['count'] -= 1;
})

$("#prevcontainer").click(function () {
    let rowIndex = ProcessInfo['index']
    let obj = Resultlist.find(obj => obj.binindex == rowIndex);
    if (ProcessInfo['binorder'] == 0) return
    RemoveElement();
    ProcessInfo['binorder'] -= 1;
    ProcessInfo['count'] = 0;
    let binorder = ProcessInfo['binorder'];
    var item = obj.information[binorder];
    document.getElementById('containertype').innerHTML = `Type ${binorder + 1} (x ${item.count})`;
    shcontainer = 1;
    var bin = obj.bininfo
    AddContainer(bin.width, bin.height, bin.depth);
    camera.position.set(bin.height * 1.5, bin.depth * 1.5, bin.width * 1.5)
    light.position.set(bin.height * 10, bin.depth * 10, bin.width * 10)
})

$("#nextcontainer").click(function () {
    let rowIndex = ProcessInfo['index']
    let obj = Resultlist.find(obj => obj.binindex == rowIndex);
    if (obj.information.length - 1 == ProcessInfo['binorder']) return
    RemoveElement();
    ProcessInfo['binorder'] += 1;
    ProcessInfo['count'] = 0;
    let binorder = ProcessInfo['binorder'];
    var item = obj.information[binorder];
    document.getElementById('containertype').innerHTML = `Type ${binorder + 1} (x ${item.count})`;
    shcontainer = 1;
    var bin = obj.bininfo
    AddContainer(bin.width, bin.height, bin.depth);
    camera.position.set(bin.height * 1.5, bin.depth * 1.5, bin.width * 1.5)
    light.position.set(bin.height * 10, bin.depth * 10, bin.width * 10)
})

$("#AddContainer").click(function () {
    var containercount = document.getElementById("ContainerInfo").rows.length;
    $('#ContainerInfo').append($('<tr>')
        .append($('<td>').append(`<div class="countcol">${containercount}</div>`))
        .append($('<td>').append(`<input placeholder="name" class="inputfield ContainerName" type="text" />`))
        .append($('<td>').append(`<input placeholder="width" class="inputfield ContainerWidth" type="number" />`))
        .append($('<td>').append(`<input placeholder="height" class="inputfield ContainerHeight" type="number" />`))
        .append($('<td>').append(`<input placeholder="depth" class="inputfield ContainerDepth" type="number" />`))
        .append($('<td>').append(`<input placeholder="max weight" class="inputfield ContainerMaxWeight" type="number" />`))
        .append($('<td>').append(`<div class="resultview" data-bs-toggle="modal" data-bs-target="#PackedContainer"><div class="resultbtntext">Result</div></div>`))
        .append($('<td>').append(`<img class="deleterow" src="/SVG/delete.png">`))
    )
});

$("#AddCargo").click(function () {
    var cargocount = document.getElementById("CargoInfo").rows.length;
    $('#CargoInfo').append($('<tr>')
        .append($('<td>').append(`<div class="countcol">${cargocount}</div>`))
        .append($('<td>').append(`<input placeholder="name" class="inputfield CargoName" type="text" />`))
        .append($('<td>').append(`<input placeholder="width" class="inputfield CargoWidth" type="number" />`))
        .append($('<td>').append(`<input placeholder="height" class="inputfield CargoHeight" type="number" />`))
        .append($('<td>').append(`<input placeholder="depth" class="inputfield CargoDepth" type="number" />`))
        .append($('<td>').append(`<input placeholder="weight" class="inputfield CargoWeight" type="number" />`))
        .append($('<td>').append(`<input placeholder="count" class="inputfield CargoCount" type="number" />`))
        .append($('<td>').append(`<select class="inputfield CargoRotationInfo" "><option value="all">All</option><option value="depth">By Depth(regular)</option><option value="width">By Width</option><option value="height">By Height</option></select>`))
        .append($('<td>').append(`<img class="deleterow" src="/SVG/delete.png">`))
    )

    $(`table#CargoInfo tr:nth-child(${cargocount + 1}) .CargoRotationInfo`).change(Rerender);
});

$("#CalculatePacking").click(function () {
    CalculateStart();
    CalculatePacking();
})

$("#setset").click(function () {
    let container = Testdata['Container'];
    let cargo = Testdata['Cargo']
    for (var cont = 0; cont < container.length; cont++) {
        $(`table#ContainerInfo tr:nth-child(${cont + 2}) .ContainerName`).val(container[cont]['name'])
        $(`table#ContainerInfo tr:nth-child(${cont + 2}) .ContainerWidth`).val(container[cont]['width'])
        $(`table#ContainerInfo tr:nth-child(${cont + 2}) .ContainerHeight`).val(container[cont]['height'])
        $(`table#ContainerInfo tr:nth-child(${cont + 2}) .ContainerDepth`).val(container[cont]['depth'])
        $(`table#ContainerInfo tr:nth-child(${cont + 2}) .ContainerMaxWeight`).val(container[cont]['maxweight'])
    }
    for (var car = 0; car < cargo.length; car++) {
        $(`table#CargoInfo tr:nth-child(${car + 2}) .CargoName`).val(cargo[car]['name'])
        $(`table#CargoInfo tr:nth-child(${car + 2}) .CargoWidth`).val(cargo[car]['width'])
        $(`table#CargoInfo tr:nth-child(${car + 2}) .CargoHeight`).val(cargo[car]['height'])
        $(`table#CargoInfo tr:nth-child(${car + 2}) .CargoDepth`).val(cargo[car]['depth'])
        $(`table#CargoInfo tr:nth-child(${car + 2}) .CargoWeight`).val(cargo[car]['weight'])
        $(`table#CargoInfo tr:nth-child(${car + 2}) .CargoCount`).val(cargo[car]['count'])
        $(`table#CargoInfo tr:nth-child(${car + 2}) .CargoRotationInfo`).val(cargo[car]['rotation'])
    }
})

function RemoveElement() {
    for (var i = scene.children.length - 1; i >= 1; i--) {
        obj = scene.children[i];
        scene.remove(obj);
    }
}

function init() {

    camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 10000);
    camera.lookAt(0, 0, 0);

    scene = new THREE.Scene();
    scene.background = new THREE.Color(0xf0f0f0);
    scene.add(camera)

    ProcessInfo['index'] = 0;
    ProcessInfo['binorder'] = 0;
    ProcessInfo['count'] = 0;

    // const axesHelper = new THREE.AxesHelper( 100 );
    // scene.add( axesHelper );

    // lights
    LightSetup();

    textureloader = new THREE.TextureLoader()
    const boxtexture = textureloader.load('/textures/box.jpg')
    cargomaterial = new THREE.MeshBasicMaterial({ map: boxtexture });

    const pallettexture = textureloader.load('/textures/pallet.jpg')
    palletmaterial = new THREE.MeshBasicMaterial({ map: pallettexture });

    // cargomaterial = new THREE.MeshNormalMaterial({ transparent: true, opacity: 0.6 });

    renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setClearColor(0xf0f0f0);
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(window.innerWidth * 0.8, window.innerHeight * 0.8);

    controls = new THREE.OrbitControls(camera, renderer.domElement);

    $('#PackedResult').append(renderer.domElement);

    window.addEventListener('resize', onWindowResize, false);

    animate();
}

function CalculatePacking() {
    var data = {}
    var containerinfo = []
    var cargoinfo = []
    var containercount = document.getElementById("ContainerInfo").rows.length;
    for (var cont = 2; cont <= containercount; cont++) {
        var onecontainer = {}

        onecontainer['ContainerName'] = $(`table#ContainerInfo tr:nth-child(${cont}) .ContainerName`).val()
        onecontainer['ContainerWidth'] = $(`table#ContainerInfo tr:nth-child(${cont}) .ContainerWidth`).val()
        onecontainer['ContainerHeight'] = $(`table#ContainerInfo tr:nth-child(${cont}) .ContainerHeight`).val()
        onecontainer['ContainerDepth'] = $(`table#ContainerInfo tr:nth-child(${cont}) .ContainerDepth`).val()
        onecontainer['ContainerMaxWeight'] = $(`table#ContainerInfo tr:nth-child(${cont}) .ContainerMaxWeight`).val()
        onecontainer['ContainerIndex'] = cont - 1
        containerinfo.push(onecontainer)
    }
    var cargocount = document.getElementById("CargoInfo").rows.length;
    for (var i = 2; i <= cargocount; i++) {
        var onecargo = {}

        onecargo['CargoName'] = $(`table#CargoInfo tr:nth-child(${i}) .CargoName`).val()
        onecargo['CargoWidth'] = $(`table#CargoInfo tr:nth-child(${i}) .CargoWidth`).val()
        onecargo['CargoHeight'] = $(`table#CargoInfo tr:nth-child(${i}) .CargoHeight`).val()
        onecargo['CargoDepth'] = $(`table#CargoInfo tr:nth-child(${i}) .CargoDepth`).val()
        onecargo['CargoWeight'] = $(`table#CargoInfo tr:nth-child(${i}) .CargoWeight`).val()
        onecargo['CargoCount'] = $(`table#CargoInfo tr:nth-child(${i}) .CargoCount`).val()
        onecargo['CargoRotationInfo'] = $(`table#CargoInfo tr:nth-child(${i}) .CargoRotationInfo`).val()
        cargoinfo.push(onecargo)
    }
    data['ContainerInfo'] = containerinfo
    data['CargoInfo'] = cargoinfo
    $.ajax({
        url: '/Home/CalculatePacking',
        type: "POST",
        dataType: "json",
        data: data,
        success: function (data) {
            console.log(data);
            Resultlist = data;
            Resultlist.forEach(tempfunc);
            SetAdditionalInfo();
            CalculateComplete();
        },
    });
}

function tempfunc(info) {
    info.fitlist = info.information[0].fitlist;
    info.binvolusage = info.information[0].binvolusage;
    info.itemvolusage = info.information[0].itemvolusage;
    info.count = info.information[0].count;
}

function SetAdditionalInfo() {
    $('#AdditionalTable tr:gt(0)').remove();
    let containercount = Resultlist.length
    for (i = 0; i < containercount; i++) {
        let obj = Resultlist.find(obj => obj.binindex == i + 1)
        let containervolusage = parseFloat(obj.binvolusage)
        let cargovolusage = parseFloat(obj.itemvolusage)
        let count = obj.count;
        let itemcount = obj.fitlist.length
        let unfititemcount = 0;
        $('#AdditionalTable').append($('<tr style="font-size: 17px; margin-top: 5px" class="infolabel">')
            .append($('<td>').append(`${i + 1}`))
            .append($('<td>').append(`${containervolusage.toFixed(2)}%`))
            .append($('<td>').append(`${cargovolusage.toFixed(2)}%`))
            .append($('<td>').append(`${itemcount}`))
            .append($('<td>').append(`${unfititemcount}`))
            .append($('<td>').append(`${count}`))
        )
    }
}

function AddContainer(width, height, depth) {
    containergeometry = new THREE.BoxGeometry(height, depth, width);
    const containertexture = textureloader.load('/textures/container_NormalMap.png')
    containermaterial = new THREE.MeshStandardMaterial({
        color: new THREE.Color(0xff0000),
        opacity: 0.5,
        transparent: true,
        normalMap: containertexture,
    })
    const container = new THREE.Mesh(containergeometry, containermaterial)
    container.name = "container"
    container.position.set(0, 0, 0)
    scene.add(container)
    return container
}

function AddCargo(item, count) {
    const cargogeometry = new THREE.BoxGeometry(item.height, item.depth, item.width);
    const cargo = new THREE.Mesh(cargogeometry, cargomaterial)
    var posX = parseFloat(item.position[1]) + parseFloat(item.height / 2) + parseFloat(origin[1])
    var posY = parseFloat(item.position[2]) + parseFloat(item.depth / 2) + parseFloat(origin[2])
    var posZ = parseFloat(item.position[0]) + parseFloat(item.width / 2) + parseFloat(origin[0])
    cargo.position.set(posX, posY, posZ)
    cargo.name = `${count}`
    scene.add(cargo)
    return cargo
}

function LightSetup() {
    var light = new THREE.PointLight(0xffffff);
    light.position.set(0, 150, 100);
    light.name = "light"
    scene.add(light);
}

function onWindowResize() {

    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();

    renderer.setSize(window.innerWidth * 0.8, window.innerHeight * 0.8);
}

function animate() {

    requestAnimationFrame(animate);

    controls.update();

    renderer.render(scene, camera);
}

function SetPallet(width, height, depth) {
    let palleth = height / 9
    let palletw = width / 7
    pallethorizontal = new THREE.BoxGeometry(palleth, 0.5, width)
    palletvertical = new THREE.BoxGeometry(height, 0.5, palletw)
    palletmiddle = new THREE.BoxGeometry(palleth, 1.5, palletw)
    palletbottom = new THREE.BoxGeometry(palleth, 0.5, width)
    for (let i = 1; i <= 5; i++) {
        sethorizontalpallet(i, 2 * palleth, depth);
    }
    for (let j = 1; j <= 3; j++) {
        setverticalpallet(j, 3 * palletw, depth);
    }
    for (let xi = 1; xi <= 3; xi++) {
        for (let yi = 1; yi <= 3; yi++) {
            setpalletmiddle(xi, yi, 4 * palleth, 3 * palletw, depth);
        }
    }
    for (let j = 1; j <= 3; j++) {
        setpalletbottom(j, 4 * palleth, depth);
    }
}

function sethorizontalpallet(index, unit, depth) {
    let palleth = new THREE.Mesh(pallethorizontal, palletmaterial);
    palleth.name = `palleth${index}`;
    palleth.position.set(unit * (index - 3), -0.25 - depth / 2, 0)
    scene.add(palleth)
}

function setverticalpallet(index, unit, depth) {
    let palletv = new THREE.Mesh(palletvertical, palletmaterial);
    palletv.name = `palletv${index}`;
    palletv.position.set(0, -0.75 - depth / 2, unit * (index - 2))
    scene.add(palletv)
}

function setpalletmiddle(xi, yi, unitx, unity, depth) {
    let palletm = new THREE.Mesh(palletmiddle, palletmaterial)
    palletm.name = `palletm${xi}-${yi}`;
    palletm.position.set(unitx * (xi - 2), -1.75 - depth / 2, unity * (yi - 2))
    scene.add(palletm)
}

function setpalletbottom(index, unit, depth) {
    let palletb = new THREE.Mesh(palletbottom, palletmaterial)
    palletb.name = `palletb${index}`;
    palletb.position.set(unit * (index - 2), -2.75 - depth / 2, 0)
    scene.add(palletb)
}

function RemovePallet() {
    for (let i = 1; i <= 5; i++) {
        obj = scene.children.find(obj => obj.name == `palleth${i}`)
        scene.remove(obj);
    }
    for (let j = 1; j <= 3; j++) {
        obj = scene.children.find(obj => obj.name == `palletv${j}`)
        scene.remove(obj);
    }
    for (let xi = 1; xi <= 3; xi++) {
        for (let yi = 1; yi <= 3; yi++) {
            obj = scene.children.find(obj => obj.name == `palletm${xi}-${yi}`)
            scene.remove(obj);
        }
    }
    for (let j = 1; j <= 3; j++) {
        obj = scene.children.find(obj => obj.name == `palletb${j}`)
        scene.remove(obj);
    }
}

function CalculateStart() {
    $(".resultview").css("pointer-events", "none");
    $(".resultview").css("opacity", "0.2");
    $("#completeinfo").css("display", "none");
    //$(".resultview").css("cursor", "not allowed");
}

function CalculateComplete() {
    $(".resultview").css("pointer-events", "auto");
    $(".resultview").css("opacity", "1");
    $("#completeinfo").css("display", "inline");
    //$(".resultview").css("cursor", "auto");
}