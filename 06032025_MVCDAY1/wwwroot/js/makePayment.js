
// Step 2: When user clicks Proceed to Payment
function makePayment(addressid) {
    
    console.log("Proceeding with Address ID:", addressid);

    var orderItems = [];
    var totalAmount = 0;
    var isFromCart = false; // <-- Default
    if ($(".cart-item").length > 0) {
        isFromCart = true; // <-- It's a cart purchase
        $(".cart-item").each(function () {
            var productId = $(this).data('product-id');
            var quantity = parseInt($(this).find('.cart-item-quantity').text());
            var price = parseFloat($(this).find('.cart-item-price').text());

            totalAmount += price * quantity;

            orderItems.push({
                ProductId: productId,
                Quantity: quantity,
                Price: price
            });
        });
    } else {
        var amt = $("#amountInput").val();
        var productId = $("#productIdInput").val();
        var quantity = parseInt($("#quantityInput").val());
        var price = parseFloat(amt);

        totalAmount = price * quantity;

        orderItems.push({
            ProductId: parseInt(productId),
            Quantity: quantity,
            Price: price
        });
    }

    // Step 1: Initiate Razorpay Order
    fetch('/Payment/InitiateOrder', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            Amount: totalAmount,
            orderItems: orderItems,
            AddressId: addressid
        })
    })
        .then(response => response.json())
        .then(data => {
            if (data.orderId) {
                var options = {
                    "key": "rzp_test_A6DIgBxiN6cygo",
                    "amount": totalAmount * 100,
                    "currency": "INR",
                    "name": "Your Company",
                    "description": "Order Payment",
                    "order_id": data.orderId,
                    "handler": function (response) {
                        var paymentData = {
                            razorpay_payment_id: response.razorpay_payment_id,
                            razorpay_order_id: response.razorpay_order_id,
                            razorpay_signature: response.razorpay_signature,
                            amount: totalAmount,
                            addressId: addressid,
                            isFromCart: isFromCart // <-- THIS LINE ADDED
                        };
                        console.log(paymentData);
                        fetch('/Payment/Success', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify(paymentData)
                        })
                            .then(res => res.json())
                            .then(response => {
                                if (response.success) {
                                    alert('Payment successful!');
                                    window.location.href = '/Payment/ThankYou';
                                } else {
                                    alert('Payment verification failed');
                                }
                            });
                    },
                    "prefill": {
                        "name": "Test User",
                        "email": "test@example.com", 
                        "contact": "9909817574"
                    }
                };

                var rzp1 = new Razorpay(options);
                rzp1.open();
            } else {
                alert('Error: ' + data.error);
            }
        });
}
