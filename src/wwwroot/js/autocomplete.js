"use strict";

var onOutsideClickFunctions = {};

window.autocomplete = {
    assemblyname: "autocomplete",
    setFocus: function (element) {
        if (element && element.focus) element.focus();
    },
    // No need to remove the event listeners later, the browser will clean this up automagically.
    addKeyDownEventListener: function (element) {
        if (element) {
            element.addEventListener('keydown', function (event) {
                var key = event.key;

                if (key === "Enter") {
                    event.preventDefault();
                }
            });
        }
    },
    onOutsideClickClear: function (element) {
        if (element == null) {
            return;
        }

        var bId = "";
        for (var clearCount = 0; clearCount < element.attributes.length; clearCount++) {
            var a = element.attributes[clearCount];
            if (a.name.startsWith('_bl_')) {
                bId = a.name;
                break;
            }
        }

        var func = onOutsideClickFunctions[bId];
        if (func == null || func == "undefined") {
            return;
        }

        window.removeEventListener("click", func);
        onOutsideClickFunctions[bId] = null;
    },
    onOutsideClick: function (searchTextElement, dotnetRef, methodName, clearOnFire) {
        if (searchTextElement == null) {
            return;
        }

        // get the blazor internal ID to distinguish different components
        var bId = "";
        for (var clearCount = 0; clearCount < searchTextElement.attributes.length; clearCount++) {
            var a = searchTextElement.attributes[clearCount];
            if (a.name.startsWith('_bl_')) {
                bId = a.name;
                break;
            }
        }

        // clean up just in case
        autocomplete.onOutsideClickClear(searchTextElement);

        var func = function (e) {
            var parent = e.target;
            while (parent != null) {
                if (parent.classList != null && parent.classList.contains('autocomplete')) {
                    // check if this is the same typeahead parent element
                    var hasSearch = parent.contains(searchTextElement); 
                    if (hasSearch) {
                        // still in the search so don't fire
                        return;
                    }
                }
                parent = parent.parentNode;
            }

            dotnetRef.invokeMethodAsync(methodName);

            // could also add a check to see if the search element is missing on the DOM to force cleaning up the function?
            if (clearOnFire) {
                AutoComplete.onOutsideClickClear(searchTextElement);
            }
        };

        // save a reference to the click function
        onOutsideClickFunctions[bId] = func;
        window.addEventListener("click", func);
    }
};