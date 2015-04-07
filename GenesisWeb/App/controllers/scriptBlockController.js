Genesis.ScriptBlockController = Ember.ObjectController.extend({
    needs: "application",   
    application: Ember.computed.alias("controllers.application"),   
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
              "/api/script/saveconfigurations", {
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
        }
    }
})