using Godot.Collections;
using Godot;

namespace AndroidInAppPurchasesWithCSharp.GodotGooglePlayBilling
{
    public enum PurchaseType
    {
        InApp,
        Subs,
    }

    public partial class GooglePlayBilling : Node
    {
        [Signal] public delegate void ConnectedEventHandler();
        [Signal] public delegate void DisconnectedEventHandler();
        [Signal] public delegate void ConnectErrorEventHandler(int code, string message);
        [Signal] public delegate void SkuDetailsQueryCompletedEventHandler(Array skuDetails);
        [Signal] public delegate void SkuDetailsQueryErrorEventHandler(int code, string message, string[] querySkuDetails);
        [Signal] public delegate void PurchasesUpdatedEventHandler(Array purchases);
        [Signal] public delegate void PurchaseErrorEventHandler(int code, string message);
        [Signal] public delegate void PurchaseAcknowledgedEventHandler(string purchaseToken);
        [Signal] public delegate void PurchaseAcknowledgementErrorEventHandler(int code, string message);
        [Signal] public delegate void PurchaseConsumedEventHandler(string purchaseToken);
        [Signal] public delegate void PurchaseConsumptionErrorEventHandler(int code, string message, string purchaseToken);

        [Export] public bool AutoReconnect { get; set; }
        [Export] public bool AutoConnect { get; set; }

        public bool IsAvailable { get; private set; }

        private GodotObject _payment;

        public override void _Ready()
        {
            if (Engine.HasSingleton("GodotGooglePlayBilling"))
            {
                IsAvailable = true;
                _payment = Engine.GetSingleton("GodotGooglePlayBilling");
                // These are all signals supported by the API
                // You can drop some of these based on your needs
                // _payment.Connect("connected", this, nameof(OnGodotGooglePlayBilling_connected)); // No params
                // _payment.Connect("disconnected", this, nameof(OnGodotGooglePlayBilling_disconnected)); // No params
                // _payment.Connect("connect_error", this, nameof(OnGodotGooglePlayBilling_connect_error)); // Response ID (int), Debug message (string)
                // _payment.Connect("sku_details_query_completed", this, nameof(OnGodotGooglePlayBilling_sku_details_query_completed)); // SKUs (Array of Dictionary)
                // _payment.Connect("sku_details_query_error", this, nameof(OnGodotGooglePlayBilling_sku_details_query_error)); // Response ID (int), Debug message (string), Queried SKUs (string[])
                // _payment.Connect("purchases_updated", this, nameof(OnGodotGooglePlayBilling_purchases_updated)); // Purchases (Array of Dictionary)
                // _payment.Connect("purchase_error", this, nameof(OnGodotGooglePlayBilling_purchase_error)); // Response ID (int), Debug message (string)
                // _payment.Connect("purchase_acknowledged", this, nameof(OnGodotGooglePlayBilling_purchase_acknowledged)); // Purchase token (string)
                // _payment.Connect("purchase_acknowledgement_error", this, nameof(OnGodotGooglePlayBilling_purchase_acknowledgement_error)); // Response ID (int), Debug message (string), Purchase token (string)
                // _payment.Connect("purchase_consumed", this, nameof(OnGodotGooglePlayBilling_purchase_consumed)); // Purchase token (string)
                // _payment.Connect("purchase_consumption_error", this, nameof(OnGodotGooglePlayBilling_purchase_consumption_error)); // Response ID (int), Debug message (string), Purchase token (string)
                
                _payment.Connect("connected", new Callable(this, nameof(OnGodotGooglePlayBilling_connected))); // No params
                _payment.Connect("disconnected", new Callable(this, nameof(OnGodotGooglePlayBilling_disconnected))); // No params
                _payment.Connect("connect_error", new Callable(this, nameof(OnGodotGooglePlayBilling_connect_error))); // Response ID (int), Debug message (string)
                _payment.Connect("sku_details_query_completed", new Callable(this, nameof(OnGodotGooglePlayBilling_sku_details_query_completed))); // SKUs (Array of Dictionary)
                _payment.Connect("sku_details_query_error", new Callable(this, nameof(OnGodotGooglePlayBilling_sku_details_query_error))); // Response ID (int), Debug message (string), Queried SKUs (string[])
                _payment.Connect("purchases_updated", new Callable(this, nameof(OnGodotGooglePlayBilling_purchases_updated))); // Purchases (Array of Dictionary)
                _payment.Connect("purchase_error", new Callable(this, nameof(OnGodotGooglePlayBilling_purchase_error))); // Response ID (int), Debug message (string)
                _payment.Connect("purchase_acknowledged", new Callable(this, nameof(OnGodotGooglePlayBilling_purchase_acknowledged))); // Purchase token (string)
                _payment.Connect("purchase_acknowledgement_error", new Callable(this, nameof(OnGodotGooglePlayBilling_purchase_acknowledgement_error))); // Response ID (int), Debug message (string), Purchase token (string)
                _payment.Connect("purchase_consumed", new Callable(this, nameof(OnGodotGooglePlayBilling_purchase_consumed))); // Purchase token (string)
                _payment.Connect("purchase_consumption_error", new Callable(this, nameof(OnGodotGooglePlayBilling_purchase_consumption_error))); // Response ID (int), Debug message (string), Purchase token (string)
            }
            else
            {
                IsAvailable = false;
            }
        }

        #region GooglePlayBilling Methods

        public void StartConnection() => _payment?.Call("startConnection");

        public void EndConnection() => _payment?.Call("endConnection");

        public void QuerySkuDetails(string[] querySkuDetails, PurchaseType type) => _payment?.Call("querySkuDetails", querySkuDetails, $"{type}".ToLower());

        public bool IsReady() => ((bool?)_payment?.Call("isReady")) ?? false;

        public void AcknowledgePurchase(string purchaseToken) => _payment?.Call("acknowledgePurchase", purchaseToken);

        public void ConsumePurchase(string purchaseToken) => _payment?.Call("consumePurchase", purchaseToken);

        public BillingResult Purchase(string sku)
        {
            if (_payment == null) return null;
            var result = (Dictionary)_payment.Call("purchase", sku);
            return new BillingResult(result);
        }

        public PurchasesResult QueryPurchases(PurchaseType purchaseType)
        {
            if (_payment == null) return null;
            var result = (Dictionary)_payment.Call("queryPurchases", $"{purchaseType}".ToLower());
            return new PurchasesResult(result);
        }

        #endregion

        #region GodotGooglePlayBilling Signals

        private void OnGodotGooglePlayBilling_connected() => EmitSignal(nameof(ConnectedEventHandler));

        private void OnGodotGooglePlayBilling_disconnected() => EmitSignal(nameof(DisconnectedEventHandler));

        private void OnGodotGooglePlayBilling_connect_error(int code, string message) => EmitSignal(nameof(ConnectErrorEventHandler), code, message);

        private void OnGodotGooglePlayBilling_sku_details_query_completed(Array skuDetails) => EmitSignal(nameof(SkuDetailsQueryCompletedEventHandler), skuDetails);

        private void OnGodotGooglePlayBilling_sku_details_query_error(int code, string message, string[] querySkuDetails) => EmitSignal(nameof(SkuDetailsQueryErrorEventHandler), code, message, querySkuDetails);

        private void OnGodotGooglePlayBilling_purchases_updated(Array purchases) => EmitSignal(nameof(PurchasesUpdatedEventHandler), purchases);

        private void OnGodotGooglePlayBilling_purchase_error(int code, string message) => EmitSignal(nameof(PurchaseErrorEventHandler), code, message);

        private void OnGodotGooglePlayBilling_purchase_acknowledged(string purchaseToken) => EmitSignal(nameof(PurchaseAcknowledgedEventHandler), purchaseToken);

        private void OnGodotGooglePlayBilling_purchase_acknowledgement_error(int code, string message) => EmitSignal(nameof(PurchaseAcknowledgementErrorEventHandler), code, message);

        private void OnGodotGooglePlayBilling_purchase_consumed(string purchaseToken) => EmitSignal(nameof(PurchaseConsumedEventHandler), purchaseToken);

        private void OnGodotGooglePlayBilling_purchase_consumption_error(int code, string message, string purchaseToken) => EmitSignal(nameof(PurchaseConsumptionErrorEventHandler), code, message, purchaseToken);

        #endregion
    }
}
