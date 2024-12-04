function detectTabCloseF(dotNetInstance) {
    window.addEventListener("beforeunload", () => {
        dotNetInstance.invokeMethodAsync("OnTabClosing")
            .catch(err => console.error("Error invoking OnTabClosing:", err));
    });
}
