import { useEffect, useState } from "react";
import "./App.css";
import Api from "./api/api";
import logo from "./assets/logo.png";

import { TransactionContext, PosClientIdContext } from "./context";

import Pos from "./components/Pos/Pos";
import TransactionList from "./components/TransactionList/TransactionList";

function App() {
  const [posClientId, setPosClientId] = useState(0);
  const [transactions, setTransactions] = useState<TransactionSummary[]>([]);
  const [activeTransactionId, setActiveTransactionId] = useState(-1);

  useEffect(() => {
    setPosClientId(1);
  }, []);

  useEffect(() => {
    if (posClientId > 0) {
      getActiveTransactions(posClientId);
    }
  }, [posClientId]);

  const getActiveTransactions = async (posClientId: number) => {
          const api = new Api({ posClientId });
    const transactions = await api.getActiveTransactionSummaries();
    setTransactions(transactions);

    if (transactions.length > 0) setActiveTransactionId(transactions[0].id);
    else setActiveTransactionId(-1);
  };

  return (
    <div className="appContainer">
      <div className="header">
        <img src={logo}></img>
      </div>
      <div className="app">
        <PosClientIdContext.Provider value={{ posClientId, setPosClientId }}>
          <TransactionContext.Provider
            value={{ activeTransactionId, setActiveTransactionId }}
          >
            <TransactionList
              transactions={transactions}
              refreshTransactions={() => {
                getActiveTransactions(posClientId);
              }}
            />
            <Pos
              refreshTransactions={() => {
                getActiveTransactions(posClientId);
              }}
            />
          </TransactionContext.Provider>
        </PosClientIdContext.Provider>
      </div>
    </div>
  );
}

export default App;
