document.addEventListener("DOMContentLoaded", function () {
    loadDashboardCounts();
    loadVendorWarehouseDetails();

    function loadDashboardCounts() {
        fetch('/Supplier/GetDashboardCounts')
            .then(response => response.json())
            .then(data => {
                document.getElementByClass("totalOrders").innerText = data.totalOrders;
                document.getElementByClass("vendorCount").innerText = data.vendorCount;
                document.getElementByClass("warehouseCount").innerText = data.warehouseCount;
            })
            .catch(error => {
                console.error("Error fetching dashboard counts:", error);
            });
    }

    //function loadVendorWarehouseDetails() {
    //    fetch('/Dashboard/GetVendorWarehouseDetails')
    //        .then(response => response.json())
    //        .then(data => {
    //            const tableBody = document.getElementById('vendorWarehouseTable');
    //            tableBody.innerHTML = '';

    //            data.forEach(item => {
    //                const row = `
    //                    <tr>
    //                        <td>${item.vendorName}</td>
    //                        <td>${item.itemAvailable}</td>
    //                        <td>${item.warehouseName}</td>
    //                    </tr>`;
    //                tableBody.innerHTML += row;
    //            });
    //        })
    //        .catch(error => {
    //            console.error("Error fetching vendor-warehouse details:", error);
    //        });
    //}
});       
