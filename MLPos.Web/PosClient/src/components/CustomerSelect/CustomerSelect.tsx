import "./CustomerSelect.css";
import { useEffect, useState } from "react";
import Api from "../../api/api";
import leftArrow from "../../assets/icons/left-arrow.png";

function CustomerSelect({ onSelectCustomer }: CustomerSelectProps) {
  const [customers, setCustomers] = useState<Customer[]>([]);

  const getAllCustomers = async () => {
    const api = new Api({ posClientId: -1 });
    const customers = await api.getAllCustomers();
    setCustomers(customers);
  };

  useEffect(() => {
    getAllCustomers();
  }, []);

  return (
    <div className="modal">
      <div className="modalContent customerSelection">
        <img
          className="back"
          src={leftArrow}
          onClick={() => {
            onSelectCustomer(-1);
          }}
        />
        <div className="customerList">
          {customers.map((customer: Customer) => {
            return (
              <div
                className="customerCard"
                key={customer.id}
                onClick={() => onSelectCustomer(customer.id)}
              >
                <div className="customerImageContainer">
                  <img src={customer.image}></img>
                </div>
                <div className="customerInfoContainer">
                  <div className="customerName">{customer.name}</div>
                  <div className="customerEmail">{customer.email}</div>
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}

export default CustomerSelect;
