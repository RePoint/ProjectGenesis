Genesis.ScriptLinksView = Ember.View.extend({

    didInsertElement: function () {
        $("#newScriptLinksForm").validate();
        $("#updateScriptLinksForm").validate();        
    }

});