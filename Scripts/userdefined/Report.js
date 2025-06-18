
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("previewBtn").addEventListener("click", function (e) {
        e.preventDefault();
        const selectedValue = document.getElementById("reportType").value;
        if (selectedValue) {
            const url = reportBaseUrl + "/" + selectedValue;
            document.getElementById("reportPreview").src = url;
        }
    });
}); 