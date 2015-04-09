Genesis.notAdmin = Ember.Route.extend({
    model: function () {
        return Em.$.ajax(
          "/api/script/getconfigurations", {
              data: "{ 'SPHostUrl': '" + decodeURIComponent(getQueryStringParameter('SPHostUrl')) + "', 'SPAppToken': '" + Genesis.pageData.spAppToken + "' }",
              contentType: "application/json",
              processData: false,
              type: 'POST'
          }
        );
    }
});