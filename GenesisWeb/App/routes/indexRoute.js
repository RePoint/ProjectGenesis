Genesis.IndexRoute = Ember.Route.extend({
    renderTemplate: function () {     
        this.render();    // Render default outlet   
        this.render('scriptLinks', {// the template to render
            into: 'index',              // the template to render into
            outlet: 'scriptLinks',       // the name of the outlet in that template
        });
        this.render('scriptBlock', {
            into: 'index',
            outlet: 'scriptBlock',
        });
    },
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
    setupControllers:function (controller, model) {
        this.controllerFor('scriptLinks').set('model', model);
        this.controllerFor('scriptBlock').set('model', model);
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