import { useEffect, useState } from "react";
import "./App.css";
import Api from "./api/api";

import {
  TransactionContext,
  PosClientIdContext,
  LocalizedStringsContext,
} from "./context";

import Pos from "./components/Pos/Pos";
import TransactionList from "./components/TransactionList/TransactionList";
import SingleInputForm from "./components/InputForm/SingleInputForm";
import Header from "./components/Header/Header";

function App() {
  const [posClientId, setPosClientId] = useState(0);
  const [transactions, setTransactions] = useState<TransactionSummary[]>([]);
  const [activeTransactionId, setActiveTransactionId] = useState(-1);
  const [languageId, setLanguageId] = useState(
    () => localStorage.getItem("languageId") ?? navigator.language,
  );
  const [localizedStrings, setLocalizedStrings] = useState<LocalizedStrings>();

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

  useEffect(() => {
    getLocalizedStrings(languageId);
  }, [languageId]);

  const getLocalizedStrings = async (languageId: string) => {
    const api = new Api({ posClientId: -1 });
    const strings = await api.getLocalizedStrings(languageId);
    setLocalizedStrings(strings);
  };

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

  if (!localizedStrings) {
    return <div>Loading...</div>;
  }

  if (posClientId <= 0) {
    return (
      <div className="appContainer">
        <Header
          selectedLanguage={languageId}
          onSelectLanguage={(languageId) => {
            window.localStorage.setItem("languageId", languageId);
            setLanguageId(languageId);
          }}
        />
        <SingleInputForm
          label={localizedStrings.strings["LoginCode"]}
          type="text"
          submitLabel={localizedStrings.strings["Login"]}
          onSubmit={setPosClientIdFromLoginCode}
        />
      </div>
    );
  }

  return (
    <div className="appContainer">
      <Header
        selectedLanguage={languageId}
        onSelectLanguage={(languageId) => {
          console.log(languageId);
          window.localStorage.setItem("languageId", languageId);
          setLanguageId(languageId);
        }}
      />
      <div className="app">
        <LocalizedStringsContext.Provider
          value={{ localizedStrings, setLocalizedStrings }}
        >
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
        </LocalizedStringsContext.Provider>
      </div>
    </div>
  );
}

export default App;
