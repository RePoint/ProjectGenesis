Genesis.ScriptLinksController = Ember.ObjectController.extend({
    needs: "application",   
    application: Ember.computed.alias("controllers.application"),
    rowNotInEditMode: true,
    actions: {       
        addRow: function () {
            var self = this;

            var data = new Object;
            data.Title = "";
            data.ScriptSrc = "";
            data.Sequence = "";
            data.IsEditing = true;

            var scriptLinks = self.get('ScriptLinks');
            scriptLinks.pushObject(data);
           
        },
        editRow: function (item) {
            Ember.set(item, "IsEditing", true);
            this.set("rowNotInEditMode", false);
        },      
        updateRow: function (item) {
            var self = this;

            //Clear all errros and alerts
            $(".alert").hide();
            $(".errorDetails").html("");

            if ($("#updateScriptLinksForm").valid()) {

                var postdata = {
                    SPHostUrl: decodeURIComponent(getQueryStringParameter('SPHostUrl')),
                    SPAppToken: Genesis.pageData.spAppToken,
                    ScriptLink: item
                };

                Em.$.ajax(
                  "/api/home/savescript", {
                      data: JSON.stringify(postdata),
                      contentType: "application/json",
                      processData: false,
                      type: 'POST'
                  }
                ).done(function (data) {

                    //reset the id to take care of new records
                    Ember.set(item, "Id", data.Id);

                    $("#scriptSuccess").fadeIn('slow');
                    window.setTimeout(function () {
                        $("#scriptSuccess").fadeOut('slow');
                    }, 2000);                  

                }).fail(function (jqXHR, textStatus, errorThrown) {
                    $("#scriptError").fadeIn('slow');
                    $("#scriptErrorDetails").html(jqXHR.responseJSON.Message);                  
                });

                Ember.set(item, "IsEditing", false);
                this.set("rowNotInEditMode", true);
            }
        },
        deleteRow: function (item) {
            if (confirm("Are you sure you want to delete this record?")) {
                var self = this;

                //Clear all errros and alerts
                $(".alert").hide();
                $(".errorDetails").html("");

                var postdata = {
                    SPHostUrl: decodeURIComponent(getQueryStringParameter('SPHostUrl')),
                    SPAppToken: Genesis.pageData.spAppToken,
                    ScriptLink: item
                };

                Em.$.ajax(
                  "/api/home/deletescript", {
                      data: JSON.stringify(postdata),
                      contentType: "application/json",
                      processData: false,
                      type: 'POST'
                  }
                ).done(function (data) {
                    var scriptLinks = self.get('ScriptLinks');
                    scriptLinks.removeObject(item);

                    $("#scriptSuccess").fadeIn('slow');
                    window.setTimeout(function () {
                        $("#scriptSuccess").fadeOut('slow');
                    }, 2000);

                }).fail(function (jqXHR, textStatus, errorThrown) {
                    $("#scriptError").fadeIn('slow');
                    $("#scriptErrorDetails").html(jqXHR.responseJSON.Message);
                });
            }
        }
    }
})