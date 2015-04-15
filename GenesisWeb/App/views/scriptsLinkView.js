Genesis.ScriptLinksView = Ember.View.extend({

    didInsertElement: function () {      
        $("#updateScriptLinksForm").validate();
        $(document).tooltip();
    }

});