Genesis.IndexController = Ember.ObjectController.extend({
    needs: "application",
    application: Ember.computed.alias("controllers.application"),
    setupController: function (controller, model) {      
        controller.set('model', model);
    },
    actions: {
        saveScriptBlock: function () {
            var self = this;

            //Clear all errros and alerts
            $(".alert").hide();
            $(".errorDetails").html("");

            var postdata = {
                SPHostUrl: decodeURIComponent(getQueryStringParameter('SPHostUrl')),
                SPAppToken: Genesis.pageData.spAppToken,
                configurations: this.get('model')
            };

            Em.$.ajax(
              "/api/home/saveconfigurations", {
                  data: JSON.stringify(postdata),
                  contentType: "application/json",
                  processData: false,
                  type: 'POST'
              }
            ).done(function (data) {            

                $("#scriptBlockSuccessAlert").fadeIn('slow');
                window.setTimeout(function () {                  
                    $("#scriptBlockSuccessAlert").fadeOut('slow');
                }, 2000);

            }).fail(function (jqXHR, textStatus, errorThrown) {
                $("#scriptBlockSuccessError").fadeIn('slow');
                $("#scriptBlockSuccessDetails").html(jqXHR.responseJSON.Message);
            });
        },
        addScript: function () {
            var self = this;

            //Clear all errros and alerts
            $(".alert").hide();
            $(".errorDetails").html("");

            var postdata = {
                SPHostUrl: decodeURIComponent(getQueryStringParameter('SPHostUrl')),
                SPAppToken: Genesis.pageData.spAppToken,
                Title: this.get('Title'),
                ScriptSrc: this.get('ScriptSrc'),
                Sequence: this.get('Sequence')
            };

            Em.$.ajax(
              "/api/home/addscript", {
                  data: JSON.stringify(postdata),
                  contentType: "application/json",
                  processData: false,
                  type: 'POST'
              }
            ).done(function (data) {         
                var scriptLinks = self.get('ScriptLinks');                
                scriptLinks.pushObject(data);

                $("#scriptSuccess").fadeIn('slow');
                window.setTimeout(function () {
                    $("#scriptSuccess").fadeOut('slow');
                }, 2000);

                self.set('Title', '');
                self.set('ScriptSrc', '');
                self.set('Sequence', '');

            }).fail(function (jqXHR, textStatus, errorThrown) {
                $("#scriptError").fadeIn('slow');
                $("#scriptErrorDetails").html(jqXHR.responseJSON.Message);
            });
        },
        deleteScript: function (item) {
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
})