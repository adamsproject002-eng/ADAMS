(function () {

    // 1. 備註 Modal
    function initRemarkModal() {
        const remarkModal = document.getElementById('remarkModal');
        if (!remarkModal) return;

        remarkModal.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            if (!button) return;

            const remark = button.getAttribute('data-remark') ?? "";
            const name = button.getAttribute('data-name') ?? "";

            const nameSpan = document.getElementById('remarkTargetName');
            const contentDiv = document.getElementById('remarkContent');

            if (nameSpan) nameSpan.textContent = name;
            if (contentDiv) contentDiv.textContent = remark;
        });
    }

    // 2. 通用刪除確認（範例）
    function initConfirmDelete() {
        document.querySelectorAll('[data-confirm-delete]')
            .forEach(btn => {
                btn.addEventListener('click', function (e) {
                    const msg = this.getAttribute('data-confirm-delete')
                        || '確定要刪除嗎？';
                    if (!confirm(msg)) {
                        e.preventDefault();
                    }
                });
            });
    }

    document.addEventListener('DOMContentLoaded', function () {
        initRemarkModal();
        initConfirmDelete();
        // 之後有新功能就繼續在這裡呼叫 initXxx()
    });

})();