document.addEventListener('DOMContentLoaded', function () {
    const reportType = document.getElementById("reportType");
    const batchSelectGroup = document.getElementById("batchSelectGroup");
    let selectedValue = '';
    const reportBaseUrl = window.reportBaseUrl || '/Report';

    function loadBatches() {
        const url = "/Report/LoadBatches";

        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                var $dropdown = $('#batch_no');
                $dropdown.empty();
                $dropdown.append($('<option></option>').val("").text("- All -")); // Add All option
                $.each(data, function (index, item) {
                    $dropdown.append($('<option></option>').val(item.value).text(item.text));
                });
            }
        });
    }

    function generateReport() {
        console.log("test");

        $("#reportContainer").html("");

        const selectedBatchId = $("#batch_id").val();
        const batchId = selectedBatchId !== "" ? selectedBatchId : -1;

        const url = "/Report/GenerateStatistics?batchId=" + batchId;
        const iframeTag = '<iframe src="' + url + '" id="statistics_report_id" width="100%" height="500px"></iframe>';
        $("#reportContainer").html(iframeTag);
    }

    // Preview button event handler
    document.getElementById("previewBtn").addEventListener("click", function (e) {
        e.preventDefault();
        const selectedValue = reportType.value;
        let url = reportBaseUrl + "/" + selectedValue;
        const batchId = $("#batch_no").val();
        const isByBatch = $("#reportType option:selected").data("bybatch");
        if (isByBatch && batchId) {
            url += "?batch_id=" + batchId;
        }
        document.getElementById("reportPreview").src = url;
    });

    function toggleBatchFilter() {
        const selectedOption = reportType.options[reportType.selectedIndex];
        const isByBatch = selectedOption.getAttribute("data-bybatch") === "true";
        batchSelectGroup.style.display = isByBatch ? "" : "none";
        if (isByBatch) {
            loadBatches();
        }
    }

    reportType.addEventListener("change", toggleBatchFilter);

    toggleBatchFilter(); // Initial call on page load
});