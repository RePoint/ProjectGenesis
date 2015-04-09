var EmberENV = {FEATURES: {'ember-htmlbars': true}};

window.Genesis = Ember.Application.create({
    rootElement: "#app-root"
});

//add extra properties to the text field for validations
Ember.TextField.reopen({
    attributeBindings: [
        'required',
        'digits',
        'maxlength',
        'noturl',
        'uniqueScriptTitle'
    ]
});


Genesis.ApplicationRoute = Ember.Route.extend({

    model: function () {
        return Em.$.ajax(
          "/api/user/getcurrent/", {
              data: "{ 'SPHostUrl': '" + decodeURIComponent(getQueryStringParameter('SPHostUrl')) + "', 'SPAppToken': '" + Genesis.pageData.spAppToken + "' }",
              contentType: "application/json",
              processData: false,
              type: 'POST',
              error: function (xhr, ajaxOptions, thrownError) {
                  alert(xhr.responseJSON.Message);
              }
          }
        );
    },
    setupControllers: function (controller, model) {       
        if (model.siteCollectionAdmin == false) {
            this.transitionTo('notAdmin');
        }
    }
});


Genesis.Router.map(function () {
    this.route('notAdmin');
    this.route('modelNotFound');
});


function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
          .toString(16)
          .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
      s4() + '-' + s4() + s4() + s4();
}
