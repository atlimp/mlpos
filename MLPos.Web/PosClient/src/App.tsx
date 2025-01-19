import { useEffect, useState } from "react";
import "./App.css";
import Api from "./api/api";
import logo from "./assets/logo.png";

import { TransactionContext, PosClientIdContext } from "./context";

import Pos from "./components/Pos/Pos";
import TransactionList from "./components/TransactionList/TransactionList";
import SingleInputForm from "./components/InputForm/SingleInputForm";

function App() {
  const [posClientId, setPosClientId] = useState(0);
  const [transactions, setTransactions] = useState<TransactionSummary[]>([]);
  const [activeTransactionId, setActiveTransactionId] = useState(-1);

  useEffect(() => {
    const loginCode = window.localStorage.getItem("loginCode");

    if (loginCode) {
      setPosClientIdFromLoginCode(loginCode);
    }
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

  const setPosClientIdFromLoginCode = async (loginCode: string) => {
    const api = new Api({ posClientId: -1 });

    const posClient = await api.getPosClientByLoginCode(loginCode);

    if (posClient) {
      window.localStorage.setItem("loginCode", loginCode);
      setPosClientId(posClient.id);
    }
  };

  if (posClientId <= 0) {
    return (
      <div className="appContainer">
        <div className="header">
          <img src={logo}></img>
        </div>
        <SingleInputForm
          label="Login Code"
          type="text"
          submitLabel="Login"
          onSubmit={setPosClientIdFromLoginCode}
        />
      </div>
    );
  }

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
