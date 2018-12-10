Type.registerNamespace("Sys.Extended.UI.HTMLEditor.CustomToolbarButton");

Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertIcon = function(element) {
Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertIcon.initializeBase(this, [element]);
}

Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertIcon.prototype = {

    callMethod: function() {
        if (!Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertIcon.callBaseMethod(this, "callMethod")) return false;
        this.openPopup(Function.createDelegate(this, this._onopened));
        return true;
    },

    _onopened: function(contentWindow) {
        contentWindow.insertWidget = Function.createDelegate(this, this.insertWidget);
    },

    insertWidget: function(url) {
        this.closePopup();

        var editor = this._designPanel;
        var editPanel = this._editPanel;

        try {
            // For 'Undo'
            editor._saveContent();

            // What to do - insert image at current selection
            //---------------------------------------------------
            editor.insertHTML("<eStoreWidget:User UserID=\"\">{" + url + "}</eStoreWidget:User>");
            //---------------------------------------------------

            // Notify Editor about content changed and update toolbars linked to the edit panel
            setTimeout(function() { editor.onContentChanged(); editPanel.updateToolbar(); }, 0);
            // Ensure focus in design panel
            editor.focusEditor();
            return true;
        } catch (e) { alert(e.message) }
    }
}

Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertIcon.registerClass("Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertIcon", Sys.Extended.UI.HTMLEditor.ToolbarButton.DesignModePopupImageButton);
Sys.Application.notifyScriptLoaded();
