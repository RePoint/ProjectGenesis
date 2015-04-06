Genesis.IndexController = Ember.ObjectController.extend({
    needs: "application",   
    application: Ember.computed.alias("controllers.application"),
    setupController: function (controller, model) {      
        controller.set('model', model);
    }
})