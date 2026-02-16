/**
 * QR Payment Plugin - JavaScript
 */

(function() {
    'use strict';

    // Auto-initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initQRPayment);
    } else {
        initQRPayment();
    }

    function initQRPayment() {
        // Initialize all QR payment buttons on the page
        var markPaidButtons = document.querySelectorAll('.mark-paid-btn[data-invoice-id]');
        markPaidButtons.forEach(initMarkPaidButton);
    }

    function initMarkPaidButton(button) {
        button.addEventListener('click', handleMarkPaidClick);
    }

    function handleMarkPaidClick(event) {
        event.preventDefault();
        
        var button = event.currentTarget;
        var invoiceId = button.getAttribute('data-invoice-id');
        var statusDiv = document.getElementById('qr-payment-status-' + invoiceId);
        
        if (!confirm('Are you sure you want to mark this invoice as paid?')) {
            return;
        }
        
        // Disable button and show loading state
        button.disabled = true;
        var originalContent = button.innerHTML;
        button.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Processing...';
        
        // Get auth token
        var token = '';
        if (window.btcpay && window.btcpay.token) {
            token = window.btcpay.token;
        }
        
        // Make API call
        fetch('/api/v1/plugins/qr-payment/invoices/' + invoiceId + '/mark-paid', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            },
            body: JSON.stringify({
                notes: 'Manually marked as paid via QR Payment plugin'
            })
        })
        .then(function(response) {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(function(data) {
            if (data.error) {
                showStatus(statusDiv, 'error', 'Error: ' + data.error);
                button.disabled = false;
                button.innerHTML = originalContent;
            } else {
                showStatus(statusDiv, 'paid', '<i class="fa fa-check-circle"></i> Payment confirmed on ' + new Date(data.paidAt).toLocaleString());
                button.style.display = 'none';
                
                // Trigger custom event for other scripts
                document.dispatchEvent(new CustomEvent('qrPaymentMarkedPaid', {
                    detail: {
                        invoiceId: invoiceId,
                        paidAt: data.paidAt,
                        paidBy: data.paidBy
                    }
                }));
            }
        })
        .catch(function(error) {
            console.error('Error:', error);
            showStatus(statusDiv, 'error', 'An error occurred. Please try again.');
            button.disabled = false;
            button.innerHTML = originalContent;
        });
    }

    function showStatus(container, type, message) {
        if (!container) return;
        
        var statusClass = '';
        var statusIcon = '';
        
        switch (type) {
            case 'paid':
                statusClass = 'status-paid';
                break;
            case 'error':
                statusClass = 'status-error';
                break;
            default:
                statusClass = 'status-pending';
        }
        
        container.innerHTML = '<div class="' + statusClass + '">' + message + '</div>';
    }

    // Expose global functions for manual initialization
    window.QRPayment = {
        markAsPaid: function(invoiceId, notes, callback) {
            var token = '';
            if (window.btcpay && window.btcpay.token) {
                token = window.btcpay.token;
            }
            
            fetch('/api/v1/plugins/qr-payment/invoices/' + invoiceId + '/mark-paid', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + token
                },
                body: JSON.stringify({ notes: notes || '' })
            })
            .then(function(response) { return response.json(); })
            .then(function(data) {
                if (callback) callback(data.error ? new Error(data.error) : null, data);
            })
            .catch(function(error) {
                if (callback) callback(error, null);
            });
        },
        
        getInvoiceQR: function(invoiceId, callback) {
            fetch('/api/v1/plugins/qr-payment/invoices/' + invoiceId + '/qr')
                .then(function(response) { return response.json(); })
                .then(function(data) {
                    if (callback) callback(null, data);
                })
                .catch(function(error) {
                    if (callback) callback(error, null);
                });
        }
    };
})();
