Genesis.modelNotFoundRoute = Ember.Route.extend({
    model: function () {
        return Em.$.ajax(
          "/api/home/getconfigurations", {
              data: "{ 'SPHostUrl': '" + decodeURIComponent(getQueryStringParameter('SPHostUrl')) + "', 'SPAppToken': '" + Genesis.pageData.spAppToken + "' }",
              contentType: "application/json",
              processData: false,
              type: 'POST'
          }
        );
    }    
});