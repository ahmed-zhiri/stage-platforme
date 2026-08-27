// Fonctions front-office partagées (jQuery / Bootstrap / AJAX)

(function () {
    "use strict";

    // Conteneur unique pour les notifications (toasts) déclenchées en AJAX.
    function ensureToastContainer() {
        var container = document.getElementById("toast-container");
        if (!container) {
            container = document.createElement("div");
            container.id = "toast-container";
            document.body.appendChild(container);
        }
        return container;
    }

    // Affiche une notification Bootstrap. type = success | danger | warning | info
    window.showToast = function (message, type) {
        type = type || "info";
        var container = ensureToastContainer();

        var wrapper = document.createElement("div");
        wrapper.className = "toast align-items-center text-bg-" + type + " border-0 show mb-2";
        wrapper.setAttribute("role", "alert");
        wrapper.innerHTML =
            '<div class="d-flex">' +
            '  <div class="toast-body">' + message + '</div>' +
            '  <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>' +
            '</div>';

        container.appendChild(wrapper);

        // Auto-fermeture après 3,5 s
        setTimeout(function () {
            wrapper.classList.remove("show");
            setTimeout(function () { wrapper.remove(); }, 300);
        }, 3500);
    };
})();
