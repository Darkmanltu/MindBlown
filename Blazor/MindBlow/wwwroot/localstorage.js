window.localStorageHelper = {
    saveItems: function (key, value) {
        localStorage.setItem(key, JSON.stringify(value));
    },
    getItems: function (key) {
        const items = localStorage.getItem(key);
        return items ? JSON.parse(items) : [];
    },
    clearItems: function (key) {
        localStorage.removeItem(key);
    }
};
