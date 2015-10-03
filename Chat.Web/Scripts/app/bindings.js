ko.bindingHandlers.fadeOut = {
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = valueAccessor();
        var valueUnwrapped = ko.unwrap(value);
        $(element).text(valueUnwrapped).delay(3000).fadeOut();
        if (valueUnwrapped) {
            viewModel.error(' '); // makes next error to show up correctly even if it is the same as previous one. 
        }
    }
};

ko.bindingHandlers.enterKey = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        var callback = valueAccessor();
        $(element).keypress(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode);
            if (keyCode === 13) {
                callback.call(viewModel);
                return false;
            }
            return true;
        });
    }
};

ko.bindingHandlers.hidden = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.visible.update(element, function () {
            return !ko.utils.unwrapObservable(valueAccessor());
        });
    }
};