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
            var deleteLinkSelect = document.getElementById("link-to-delete-select");
            var target = deleteLinkSelect[deleteLinkSelect.selectedIndex].innerHTML.split(" - ")[0];
            var source = deleteLinkSelect[deleteLinkSelect.selectedIndex].innerHTML.split(" - ")[1];
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
        for (var i = 5; i >= 1; i--) {
            nodes.push({"id" : i, "group" : i});
        }
        app.AjaxQuery("POST", "/GraphProvider/AddNodes", nodes, app.reload);
        app.AjaxQuery("POST", "/GraphProvider/SetTask", 1);
    })
    $("#cycle-graph-btn").click(function () {
        var nodes = [];
        var links = [];
        for (let i = 5; i >= 1; i--) {
            nodes.push({"id" : i, "group" : i});
            if (i !== 1)
            {
                links.push({"source": i, "target": i - 1, "value": 1})
            }
        }
        
        links.push({"source": links[0].source, "target": links[links.length - 1].target, "value" : 1});
        links.push({"source": 4, "target": 1 , "value" : 1});
        links.push({"source": 5, "target": 3, "value" : 1});
        links.push({"source": 5, "target": 2, "value" : 1});
        
        
        app.AjaxQuery("POST", "/GraphProvider/AddNodes", nodes);
        app.AjaxQuery("POST", "/GraphProvider/AddLinks", links, app.reload);
        app.AjaxQuery("POST", "/GraphProvider/SetTask", 2);
    });
    
    $("#check-complete-graph-btn").click(function () {
        app.checkTask(
            function(d) {
            switch(d){
                case 1:
                    app.AjaxQuery("GET", "/GraphProvider/CheckCompleteGraph", null, function (IsCompleted) {
                        if(IsCompleted)
                        {
                            app.AjaxQuery("POST","/GraphProvider/SetTask", false, function () {
                                app.alertSuccess("Nice! ", "You have completed the task!");
                                app.reset();
                                app.reload();
                            });
                        }
                        else {
                            app.alert("Mistake. ","Your graph isn't complete yet!")
                        }
                    });
                    break;
                case 2:
                    app.AjaxQuery("GET", "/GraphProvider/CheckForCycles", null, function (IsCompleted) {
                        if(IsCompleted) {
                            app.alert("Mistake. ","There are still cycles in the graph!")
                        }
                        else
                        {
                            app.AjaxQuery("POST","/GraphProvider/SetTask", false, function () {
                                app.alertSuccess("Nice! ", "You have completed the task!");
                                app.reset();
                                app.reload();
                            });
                        }
                    });
            }
        })
    })
    
    $("#reset-btn").click(function() {
            app.reset();
        }
    )

};

window.onresize = function() {
    app.reload();
};