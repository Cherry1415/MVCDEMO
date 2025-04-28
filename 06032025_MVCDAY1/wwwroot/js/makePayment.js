function makePayment() {
    var orderItems = [];
    var totalAmount = 0;

    // Check if cart items exist (for multiple items)
    if ($(".cart-item").length > 0) {
        // MULTIPLE ITEMS (From Cart)
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
        // SINGLE ITEM (Buy Now Page)
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

    console.log("Total Amount:", totalAmount);
    console.log("Order Items:", orderItems);

    // Call backend to create Razorpay order
    fetch('/Payment/InitiateOrder', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            Amount: totalAmount,
            orderItems: orderItems
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
                        console.log("Payment Response:", response);

                        var paymentData = {
                            razorpay_payment_id: response.razorpay_payment_id,
                            razorpay_order_id: response.razorpay_order_id,
                            razorpay_signature: response.razorpay_signature,
                            amount: totalAmount
                        };

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
