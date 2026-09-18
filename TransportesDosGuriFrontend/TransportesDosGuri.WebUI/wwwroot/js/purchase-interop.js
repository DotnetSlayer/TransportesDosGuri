window.purchaseInterop = {


    openInNewTab: function (url) {
        const win = window.open(url, '_blank', 'noopener,noreferrer');
        if (!win) {

            return false;
        }
        win.focus();
        return true;
    },

    onWindowFocus: function (dotNetRef) {
        if (!window.__purchaseFocusHandler__) {
            window.__purchaseFocusHandler__ = function () {

                window.__purchaseFocusRef__?.invokeMethodAsync('OnWindowFocused');
            };
            window.addEventListener('focus', window.__purchaseFocusHandler__);
        }

        window.__purchaseFocusRef__ = dotNetRef;
    },


    disposeFocusHandler: function () {
        if (window.__purchaseFocusHandler__) {
            window.removeEventListener('focus', window.__purchaseFocusHandler__);
            window.__purchaseFocusHandler__ = null;
        }
        window.__purchaseFocusRef__ = null;
    }
};