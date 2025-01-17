import "./PaymentMethodSelect.css";
import { useEffect, useState } from "react";
import Api from "../../api/api";
import leftArrow from "../../assets/icons/left-arrow.png";

function PaymentMethodSelect({
  onSelectPaymentMethod,
  replacers,
}: PaymentMethodSelectProps) {
  const [paymentmethods, setPaymentMethods] = useState<PaymentMethod[]>([]);

  const getAllPaymentMethods = async () => {
    const api = new Api({ posClientId: -1 });
    const paymentmethods = await api.getAllPaymentMethods();
    setPaymentMethods(paymentmethods);
  };

  const replaceStrings = (s: string) => {
    Object.keys(replacers).forEach((key) => {
      s = s.replace(`{${key}}`, replacers[key]);
    });

    return s;
  };

  useEffect(() => {
    getAllPaymentMethods();
  }, []);

  return (
    <div className="modal">
      <div className="modalContent paymentmethodSelection">
        <img
          className="back"
          src={leftArrow}
          onClick={() => {
            onSelectPaymentMethod(-1);
          }}
        />
        <div className="paymentmethodList">
          {paymentmethods.map((paymentmethod: PaymentMethod) => {
            return (
              <div
                className="paymentmethodCard"
                key={paymentmethod.id}
                onClick={() => onSelectPaymentMethod(paymentmethod.id)}
              >
                <div className="paymentmethodImageContainer">
                  <img src={paymentmethod.image}></img>
                </div>
                <div className="paymentmethodInfoContainer">
                  <div className="paymentmethodName">{paymentmethod.name}</div>
                  <div className="paymentmethodDescription">
                    {replaceStrings(paymentmethod.description)}
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}

export default PaymentMethodSelect;
