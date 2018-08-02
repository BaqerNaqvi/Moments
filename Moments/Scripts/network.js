var nodes, orgiNodes, selectedNode;
var edges;
function createGraph(funcNodes, funcRelations) {  

 
    network = null;

    orgiNodes = funcNodes;
    // vertical separation 
    var vertSep = 200;
    // node separation
    var nodeSep = 150;
    // node separation
    var treeSep = 250;

    // create an array with nodes
    nodes = new vis.DataSet(funcNodes);
    // create an array with edges
    edges = new vis.DataSet(funcRelations);
    // create a network
    var container = document.getElementById('orgReportingStructure');

    // provide the data in the vis format
    data = {
        nodes: nodes,
        edges: edges
    };


    var options = {
        groups: {
            useDefaultGroups: false,
            extremeFriendliness: { color: { border: 'green' }, borderWidth: 2 },
            highFriendliness: { color: { border: 'rgba(230, 38, 73, 0.78)' }, borderWidth: 2 },
            mediumFriendliness: { color: { border: 'yellow' }, borderWidth: 2 },
            lowFriendliness: { color: { border: 'red' }, borderWidth: 2},
            unknownFriendliness: { color: { border: 'black' }, borderWidth: 2},
            notAssignedFriendliness: { color: { border: 'orange' }, borderWidth: 2 }
        },
        autoResize: true,

        edges: {
            arrows: {
                to: { enabled: false, scaleFactor: 0.7 },
                middle: { enabled: true, scaleFactor: 0.4 },
                from: { enabled: false, scaleFactor: 0.5 }
            },

            hoverWidth: function (width) { return 3; },
            // label: 'hello',
            font: {
                color: 'red',
                size: 14, // px
                face: 'arial',
                background: 'none',
                strokeWidth: 2, // px
                strokeColor: '#ffffff',
                align: 'middle'
            },
            shadow: true,
            smooth: {
                enabled: true,
                type: "cubicBezier", //straightCross
                roundness: 0.5,
                forceDirection: 'vertical' //horizontal
            },

        },      // defined in the edges module.
        nodes: {
            shape: 'circularImage',
            shadow: true,
            color: {
                border: '#333132',
                background: '#e6e6e5',
                hover: {
                    border: '#333132'
                }
            }
        },
        interaction: {
            hover: true,
            navigationButtons: true,
            hoverConnectedEdges: true,
            zoomView: true
        },  // defined in the interaction module.
        //  manipulation: {...}, // defined in the manipulation module.
        physics: { enabled: false },      // defined in the physics module.
        layout: {
            randomSeed: 1,
            improvedLayout: true,
            hierarchical: {
                enabled: true,
                levelSeparation: vertSep,
                nodeSpacing: nodeSep,
                treeSpacing: treeSep,
                blockShifting: true,
                edgeMinimization: true,
                parentCentralization: true,
                direction:  'UD',       
                sortMethod: 'directed'   
            }
        }
    };


    // initialize your network!
    network = new vis.Network(container, data, options);

    network.on("selectNode", function (params) {
        if (params.nodes != undefined && params.nodes.length > 0) {
            var fullNode = searchNode(params.nodes[0]);
            selectedNode = fullNode;
            $("#selectedNode").text(fullNode.label);
            setContentDiv(fullNode);
        }       
    });

    network.on("doubleClick", function (params) {
        if (params.nodes != undefined && params.nodes.length >0) {
            var fullNode = searchNode(params.nodes[0]);
            selectedNode = fullNode;
            $("#selectedNode").text(fullNode.label);
            // viewDetails();
            setContentDiv(fullNode);
        } else {
            selectedNode = undefined;
            $("#selectedNode").text("");
        }
    });


    network.on("hoverNode", function (params) {
        network.canvas.body.container.style.cursor = 'pointer'
    });
    network.on("blurNode", function (params) {
        network.canvas.body.container.style.cursor = 'default'
    });

    network.on("hoverEdge", function (params) {
        network.canvas.body.container.style.cursor = 'pointer'
    });
    network.on("blurEdge", function (params) {
        network.canvas.body.container.style.cursor = 'default'
    });
}

function searchNode(id) {
    var obj = undefined;
    orgiNodes.forEach(function (nodeObj, index) {
        if (nodeObj.id == id) {
            obj = nodeObj;
        }
    });
    return obj;
}

$(document).ready(function () {
    var url = new URL(location.href);
    var parm = url.searchParams.get("StoryId");

    if (parm == "" || parm == undefined) {
        toastr.error("Invalid story");
        return false;
    }

    $.ajax({
        url: location.protocol+"//" + location.host + '/Moment/GetNetwrok',
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ StoryId: parm }),
        success: function (data) {
           createGraph(data.Nodes, data.Relation);
            $("#momentsCount").text(data.Nodes.length)
            $(".storyOwner").text(data.Owner);
            $(".storyName").text(data.StoryName);
            $('#storyImg').attr('src', data.Image);

            if (data.Nodes.length == 0) {
                $('#buttonDiv').show();
                $('#imgDiv').hide();
                $('#textDiv').hide();
                $('#videoDiv').hide();
                $('#audioDiv').hide();
            }
            else {
                $("#selectedNode").text(data.Nodes[0].label);
                setContentDiv(data.Nodes[0]);
            }
        },
        error: function (data, dfsd, sdfsd) {
            alert("Failed");

        }
    });
});
function setContentDiv(root) {
    $('#videoDiv iframe').attr('src', '')
    $('#imgDiv').hide();
    $('#textDiv').hide();
    $('#videoDiv').hide();
    $('#audioDiv').hide();
    $('#buttonDiv').hide();
    if (root.type == "Image") {
        $('#imgDiv').show();
        $('#imgDiv img').attr('src', root.image);
    }
    else if (root.type == "Text") {
        $('#textDiv').show();
        $('#textDiv p').text(root.Contents);
    }
    else if (root.type == "Video" || root.type == "Audio") {
        $('#videoDiv').show();
        $('#videoDiv iframe').attr('src', root.Contents);
    }
    selectedNode = root;
}

function showTextBox() {
    $('#myModal').modal('hide');
    setTimeout(function () {
        $('#textModel').modal('show');
        $("#textArea").val('').show();
        $("#videoDivWrapper").hide();
        $('#showImg').hide();
        contentType = "Text";
    }, 300);
}

function showVideoBox() {
    $('#myModal').modal('hide');
    setTimeout(function () {
        $('#textModel').modal('show');

        $("#videoDivWrapper").show();
        $('#showImg').hide();
        $('#textArea').hide();
        contentType = "Video";
        toastr.warning("Please add Youtube video url");

    }, 300);
}

function showAudioBox() {
    $('#myModal').modal('hide');
    setTimeout(function () {
        $('#textModel').modal('show');
        $("#videoDivWrapper").show();
        $('#showImg').hide();
        $('#textArea').hide();
        contentType = "Audio";
        toastr.warning("Please add SoundClound audio url");

    }, 300);
}


var contentType = "";
function showImgBox() {
    $('#myModal').modal('hide');
    $("#videoDivWrapper").hide();
    $('#imageUploadForm').click();
}

function addNewNodeBtn() {
    setTimeout(function () {
        $('#myModal').modal('show');
    }, 100);
    $('#textModel-footer').show();

    return false;
}
$("#imageUploadForm").change(function () {

    if (orgiNodes.length > 0 && (selectedNode == null || selectedNode == undefined)) {
        toastr.error("Please select any moment first");
        return false;
    }

    var id = 0;
    if (selectedNode != null && selectedNode != undefined) {
        id= selectedNode.id;
    }

        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            formData.append("imageUploadForm", file);
        }
        var url = new URL(location.href);
        var parm = url.searchParams.get("StoryId");

        formData.append('id', id);
        formData.append('story', parm);
        
        setTimeout(function () {
            toastr.success("Loading...");
        }, 100);

        $('#loadingSpan').show();
        setTimeout(function () {
            $.ajax({
                type: "POST",
                url: '/Moment/UploadImage',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    $('#myModal').modal('hide');
                    $('#loadingSpan').hide();

                    toastr.success("Data has been added");
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                },
                error: function (error) {
                    toastr.error("Something bad happended");

                }
            })

        }, 500);
 });

function saveText() {


    if (orgiNodes.length > 0 && (selectedNode == null || selectedNode == undefined)) {
        toastr.error("Please select  moment first");
        return false;
    }

    var id = 0;
    if (selectedNode != null && selectedNode != undefined) {
        id = selectedNode.id;
    }
    var contents = "";
    if (contentType == "Text") {
         contents = $('#textArea').val();
    } else if (contentType == "Video" || contentType == "Audio") {
        contents = $('#urlArea').val();
    }

    if (contents == null || contents == "" || contents.length<15) {
        toastr.error("Please add minimum 15 chars");
        return false;
    }

     var formData = new FormData();
    
     var url = new URL(location.href);
     var parm = url.searchParams.get("StoryId");

     formData.append('id', id);
     formData.append('story', parm);
     formData.append('contents', contents);
     formData.append('type', contentType);

     setTimeout(function () {
         toastr.success("Loading...");
     }, 100);

     $.ajax({
         type: "POST",
         url: '/Moment/UploadText',
         data: formData,
         dataType: 'json',
         contentType: false,
         processData: false,
         success: function (response) {
             $('#textModel').modal('hide');
             toastr.success("Data has been added");

             setTimeout(function () {
                 location.reload();
             }, 1000);
         },
         error: function (error) {
             toastr.error("Something bad happended");

         }
     })
}

function viewDetails() {
    if (selectedNode == null || selectedNode == undefined) {
        toastr.error("Please select moment first");
        return false;
    }
    $('#textModel').modal('show');
    $('#textModel-footer').hide();

    if (selectedNode.type == "Text") {
        $("#textArea").show().val(selectedNode.Contents);
        $('#showImg').hide();
    } else if (selectedNode.type == "Image") {
        $("#textArea").hide();
        $('#showImg').show().attr('src', selectedNode.image);
    }
}

var nodesTillParent = [];
function findRoot(parmObj) {
    if (parmObj != undefined){
        nodesTillParent.push(parmObj.id);
    }
    if (parmObj == undefined || parmObj.id == 0) {
        return nodesTillParent;
    }

    var obj = undefined;
    orgiNodes.forEach(function (nodeObj, index) {
        if (nodeObj.id == parmObj.parentId) {
            obj = nodeObj;
        }
    });
    return findRoot(obj);
}

function exportStoryBtn() {
    if ((selectedNode == null || selectedNode == undefined)) {
        toastr.error("Please select moment first");
        return false;
    }
    nodesTillParent = [];
    var rootLineList = findRoot(selectedNode);

    setTimeout(function () {
        toastr.success("Working...");
    }, 100);

    if (rootLineList == undefined || rootLineList.length==0) {
        toastr.error("Please select moment first....");
        return false;
    }

    $.ajax({
        url: location.protocol+"//" + location.host + '/Moment/ExportAsPdf',
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Nodes: rootLineList, StoryId: selectedNode.storyId }),
        success: function (data) {
            filename = data.fileName;
            window.open( location.protocol+"//" + location.host + '/Moment/ViewPdf?fileName=' + filename,'_blank');
        },
        error: function (data, dfsd, sdfsd) {
            alert("Failed");
        }
    });
}

var filename;