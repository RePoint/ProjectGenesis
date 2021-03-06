﻿Genesis.CssLinksController = Ember.ObjectController.extend({
    needs: "application",   
    application: Ember.computed.alias("controllers.application"), 
    actions: {       
        addCSSRow: function () {
            var self = this;

            var data = new Object;
            data.Title = "";
            data.ScriptSrc = "";
            data.Sequence = "";
            data.Excludes = "";
            data.IsEditing = true;
            data.IsNew = true;
            data.Type = "CSS";
            data.Id = guid();

            var cssLinks = self.get('CSSLinks');
            cssLinks.pushObject(data);
           
        },
        editCSSRow: function (item) {
            Ember.set(item, "IsEditing", true);
        },
        cancelCSSRow: function (item) {
            var scriptLinks = this.get('ScriptLinks');

            if (item.IsNew == true)
                scriptLinks.removeObject(item);
            else
                Ember.set(item, "IsEditing", false);
        },
        updateCSSRow: function (item) {
            var self = this;

            //Clear all errros and alerts
            $(".alert").hide();
            $(".errorDetails").html("");

            if ($("#updateCSSLinksForm").valid()) {

                var postdata = {
                    SPHostUrl: decodeURIComponent(getQueryStringParameter('SPHostUrl')),
                    SPAppToken: Genesis.pageData.spAppToken,
                    ScriptLink: item
                };

                Em.$.ajax(
                  "/api/script/savescript", {
                      data: JSON.stringify(postdata),
                      contentType: "application/json",
                      processData: false,
                      type: 'POST'
                  }
                ).done(function (data) {

                    //reset the id to take care of new records
                    Ember.set(item, "Id", data.Id);
                    Ember.set(item, "IsNew", false);
                    Ember.set(item, "IsEditing", false);

                    $("#cssSuccess").fadeIn('slow');
                    window.setTimeout(function () {
                        $("#cssSuccess").fadeOut('slow');
                    }, 2000);                  

                }).fail(function (jqXHR, textStatus, errorThrown) {
                    $("#cssError").fadeIn('slow');
                    $("#cssErrorDetails").html(jqXHR.responseJSON.Message);                  
                });              
            }
        },
        deleteCSSRow: function (item) {
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
                  "/api/script/deletescript", {
                      data: JSON.stringify(postdata),
                      contentType: "application/json",
                      processData: false,
                      type: 'POST'
                  }
                ).done(function (data) {
                    var cssLinks = self.get('CSSLinks');
                    cssLinks.removeObject(item);

                    $("#cssSuccess").fadeIn('slow');
                    window.setTimeout(function () {
                        $("#cssSuccess").fadeOut('slow');
                    }, 2000);

                }).fail(function (jqXHR, textStatus, errorThrown) {
                    $("#cssError").fadeIn('slow');
                    $("#cssErrorDetails").html(jqXHR.responseJSON.Message);
                });
            }
        },
    }
})