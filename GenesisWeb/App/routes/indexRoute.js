Genesis.IndexRoute = Ember.Route.extend({
    model: function () {
        return Em.$.ajax(
          "/api/home/getconfigurations", {
              data: "{ 'SPHostUrl': '" + decodeURIComponent(getQueryStringParameter('SPHostUrl')) + "', 'SPAppToken': '" + Genesis.pageData.spAppToken + "' }",
              contentType: "application/json",
              processData: false,
              type: 'POST'
          }
        );
    },
    actions: {
        error: function (error, transition) {

            if (error && error.status === 400) {
                // error substate and parent routes do not handle this error
                return this.transitionTo('modelNotFound');
            }

            // Return true to bubble this event to any parent route (which will go to error.hbs).
            return true;
        }
    }
});