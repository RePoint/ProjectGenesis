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
    actions: {
        showModal: function (name, content) {
            this.controllerFor(name).set('content', content);
            this.render(name, {
                into: 'application',
                outlet: 'modal'
            });
        },
        removeModal: function () {
            this.disconnectOutlet({
                outlet: 'modal',
                parentView: 'application'
            });
        }
    },
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
    }
});


Genesis.Router.map(function () {
    this.route('modelNotFound');
});


/*
Genesis.MyModalComponent = Ember.Component.extend({
    actions: {
        ok: function () {
            this.$('.modal').modal('hide');
            this.sendAction('ok');
        }
    },
    show: function () {
        this.$('.modal').modal().on('hidden.bs.modal', function () {
            this.sendAction('close');
        }.bind(this));
    }.on('didInsertElement')
});
*/


