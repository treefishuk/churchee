window.localStorageHelper = {
    save: function (key, data) {
        localStorage.setItem(key, JSON.stringify(data));
    },
    load: function (key) {
        return JSON.parse(localStorage.getItem(key));
    }
};
