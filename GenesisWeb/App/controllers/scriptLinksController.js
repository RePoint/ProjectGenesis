Genesis.ScriptLinksController = Ember.ObjectController.extend({
    needs: "application",   
    application: Ember.computed.alias("controllers.application"),   
    actions: {       
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
        editScript: function (item) {
            Ember.set(item, "IsEditing", true);
        },      
        updateScript: function (item) {
            var self = this;

            //Clear all errros and alerts
            $(".alert").hide();
            $(".errorDetails").html("");

            var postdata = {
                SPHostUrl: decodeURIComponent(getQueryStringParameter('SPHostUrl')),
                SPAppToken: Genesis.pageData.spAppToken,
                Title: item.Title,
                ScriptSrc: item.ScriptSrc,
                Sequence: item.Sequence
            };

            Em.$.ajax(
              "/api/home/updatescript", {
                  data: JSON.stringify(postdata),
                  contentType: "application/json",
                  processData: false,
                  type: 'POST'
              }
            ).done(function (data) {
              
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

            Ember.set(item, "IsEditing", false);
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