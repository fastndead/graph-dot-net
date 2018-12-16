var app;

window.onload = function () {
    app = new App();
    app.reload();
    app.checkTask();


    $("#add-node-btn").click(function (event){
        if (document.getElementById("add-node-input-id").value.trim() === "" ||
            document.getElementById("add-node-input-group").value.trim() === "")
            {
                app.alert("Error. ","Enter name and group of node");
                return;
            }
        
        var node = {"id":document.getElementById("add-node-input-id").value,"group": document.getElementById("add-node-input-group").value};
        app.AddNode(node)
    });

    $("#add-link-btn").click(function (){
        var fromSelect = document.getElementById("from-link-select");
        var toSelect = document.getElementById("to-link-select");
        try
        {
            var link = {
                "source":fromSelect[fromSelect.selectedIndex].value,
                "target": toSelect[toSelect.selectedIndex].value,
                "value": 1
            };
        }
        catch
        {
            app.alert("Error. ","Select source and target correctly");
            return
        }
        app.AddLink(link)
    });

    $("#delete-node-btn").click( function() {
        try 
        {
            let deleteNodeSelect = document.getElementById("node-to-delete-select");            
            app.deleteNode({"id": deleteNodeSelect[deleteNodeSelect.selectedIndex].value})
        }
        catch
        {
            app.alert("Error. ","Select nodes correctly");
            return
        }
    });
    
    $("#delete-link-btn").click(function () {
        try
        {
            let deleteLinkSelect = document.getElementById("link-to-delete-select");
            let target = deleteLinkSelect[deleteLinkSelect.selectedIndex].innerHTML.split(" - ")[0];
            let source = deleteLinkSelect[deleteLinkSelect.selectedIndex].innerHTML.split(" - ")[1];
        }
        catch
        {
            app.alert("Error. ","Select links correctly");
            return
        }
        
        app.deleteLink({"source": source, "target": target})
    });
    
    $("#complete-graph-btn").click(function () {
        var nodes = []
        for (var i = 3; i >= 1; i--) {
            nodes.push({"id" : i, "group" : i});
        }
        app.AjaxQuery("POST", "/GraphProvider/AddNodes", nodes, app.reload);
        app.AjaxQuery("POST", "/GraphProvider/SetTask", true);
    })
    
    $("#check-complete-graph-btn").click(function () {
        if(app.AjaxQuery("GET", "/GraphProvider/CheckCompleteGraph", null, function (d) {
            if(d)
            {
                app.AjaxQuery("POST","/GraphProvider/SetTask", false, function () {
                    app.alertSuccess("Nice! ", "You have completed the task!");
                    app.reset();
                    app.reload();
                });
            }
            else {
                app.alert("Mistake. ","Yout graph isn't complete yet!")
            }
        }));
    })
    
    $("#reset-btn").click(function() {
            app.reset();
        }
    )

};

window.onresize = function() {
    app.reload();
};

function App() {
    var path = "/GraphProvider";
    
    this.reset = function () {
        app.AjaxQuery("DELETE", "/GraphProvider/Reset", null, function () {
            app.reload()
        })  
    };
    
    this.taskStartup = function () {
        $("#check-complete-graph-btn").prop('disabled', false);
        $("#complete-graph-modal-btn").prop('disabled', true);
    }
    
    this.taskEnd = function () {
        $("#check-complete-graph-btn").prop('disabled', true);
        $("#complete-graph-modal-btn").prop('disabled', false);
    }

    this.alert = function(boldMsg, msg){
        $("#alert").removeClass();
        $("#alert").addClass("alert alert-danger");
        $("#alert").fadeIn();
        $("#error-text-bold").text(boldMsg)
        $("#error-text").text(msg);
        if (errorTimeOut !== undefined) {
            window.clearTimeout(errorTimeOut);
        }
        var errorTimeOut = window.setTimeout(function () {
            $("#alert").fadeOut(300)
        }, 3000);
    };
    
    this.alertSuccess = function(boldMsg, msg){
        $("#alert").removeClass();
        $("#alert").addClass("alert alert-success");
        $("#alert").fadeIn();
        $("#error-text-bold").text(boldMsg)
        $("#error-text").text(msg);
        if (errorTimeOut !== undefined) {
            window.clearTimeout(errorTimeOut);
        }
        var errorTimeOut = window.setTimeout(function () {
            $("#alert").fadeOut(300)
        }, 3000);
    };
    
    
    this.deleteLink = function (link) {
        app.AjaxQuery("DELETE","/GraphProvider/deleteLink",link, app.reload);

    }
    
    this.deleteNode = function (node) {
        app.AjaxQuery("DELETE", "/GraphProvider/DeleteNode", node, app.reload);
    };
    
    this.reload = function() {
        d3.select("svg").selectAll("*").remove();
        app.checkTask();
        app.reloadSelects();
        app.plot();
    };
    
    this.checkTask = function () {
        this.AjaxQuery("GET","/GraphProvider/CheckTask", null, function (d) {
            if(d)
            {
                app.taskStartup();
            }
            else {
                app.taskEnd();
            }
        });
    }
    
    this.reloadSelects = function () {

        function removeOptions(select){
            select.find('option').remove().end();
        }
        
        removeOptions($('#from-link-select'));
        removeOptions($('#to-link-select'));
        removeOptions($('#node-to-delete-select'));
        removeOptions($('#link-to-delete-select'));
        
        $.getJSON( "/GraphProvider/GetAllNodes", function( data ) {
            var items = [];
            $.each( data, function( key, val ) {
                $('<option/>').val(val.id).html(val.id).appendTo('#from-link-select');
                $('<option/>').val(val.id).html(val.id).appendTo('#to-link-select');
                $('<option/>').val(val.id).html(val.id).appendTo('#node-to-delete-select');
            });
        });
        $.getJSON( "/GraphProvider/GetAllConnectionsString", function( data ) {
            var items = [];
            $.each( data, function( key, val ) {
                $('<option/>').val(val.target + " - " + val.source).html(val.target + " - " + val.source).appendTo('#link-to-delete-select');
            });
        });
    };

    this.change = function (newPath) {
        path = newPath;
        this.reload();
    };

    this.AddNode = function (node) {
        app.AjaxQuery("POST", "/GraphProvider/AddNode", node, app.reload)
    };

    this.AddLink = function (link) {
    app.AjaxQuery("POST", "/GraphProvider/AddLink", link, app.reload);
    };

    this.plot = function() {
        var el = document.getElementById("svg-main");

        var rect = el.getBoundingClientRect(); 
        
        var svg = d3.select("svg"),
            width = +rect.width,
            height = +rect.height;

        var color = d3.scaleOrdinal(d3.schemeCategory20);

        var simulation = d3.forceSimulation()
            .force("link", d3.forceLink().id(function (d) {
                return d.id;
            }))
            .force("charge", d3.forceManyBody())
            .force("center", d3.forceCenter(width / 2, height / 2));

        d3.json(path, function (error, graph) {
            if (error) throw error;

            var link = svg.append("g")
                .attr("class", "links")
                .selectAll("line")
                .data(graph.links)
                .enter().append("line")
                .attr("stroke-width", function (d) {
                    return Math.sqrt(d.value);
                });

            var node = svg.append("g")
                .attr("class", "nodes")
                .selectAll("g")
                .data(graph.nodes)
                .enter().append("g")

            var circles = node.append("circle")
                .attr("r", 7)
                .attr("fill", function (d) {
                    return color(d.group);
                })
                .call(d3.drag()
                    .on("start", dragstarted)
                    .on("drag", dragged)
                    .on("end", dragended));

            var lables = node.append("text")
                .text(function (d) {
                    return d.id;
                })
                .attr('x', 6)
                .attr('y', 3);

            node.append("title")
                .text(function (d) {
                    return d.id;
                });

            simulation
                .nodes(graph.nodes)
                .on("tick", ticked);

            simulation.force("link")
                .distance(200)
                .links(graph.links);

            function ticked() {
                link
                    .attr("x1", function (d) {
                        return d.source.x;
                    })
                    .attr("y1", function (d) {
                        return d.source.y;
                    })
                    .attr("x2", function (d) {
                        return d.target.x;
                    })
                    .attr("y2", function (d) {
                        return d.target.y;
                    });

                node
                    .attr("transform", function (d) {
                        return "translate(" + d.x + "," + d.y + ")";
                    })
            }
        });

        function dragstarted(d) {
            if (!d3.event.active) simulation.alphaTarget(0.3).restart();
            d.fx = d.x;
            d.fy = d.y;
        }

        function dragged(d) {
            d.fx = d3.event.x;
            d.fy = d3.event.y;
        }

        function dragended(d) {
            if (!d3.event.active) simulation.alphaTarget(0);
            d.fx = null;
            d.fy = null;
        }
    };   
    
    this.AjaxQuery = function(type,url,data, callback){
        $.ajax({
            type: type,
            url: url,
            contentType: 'application/json',
            data: JSON.stringify(data),
        }).done(function (recievedData) {
            if(callback !== undefined)
            {
                callback(recievedData);
            }
            
        }).fail(function (msg) {
            view.error(msg)
        });
    };

};
    