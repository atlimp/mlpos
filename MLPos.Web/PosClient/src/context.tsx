import { Dispatch, createContext, SetStateAction } from "react";

export interface PosClientIdContextInterface {
  posClientId: number;
  setPosClientId: Dispatch<SetStateAction<number>>;
}
export interface TransactionContextInterface {
  activeTransactionId: number;
  setActiveTransactionId: Dispatch<SetStateAction<number>>;
}
export interface LocalizedStringsContextInterface {
  localizedStrings: LocalizedStrings;
  setLocalizedStrings: Dispatch<SetStateAction<LocalizedStrings | undefined>>;
}

const PosClientIdContextDefaultState = {
  posClientId: -1,
  setPosClientId: () => {},
} as PosClientIdContextInterface;

const TransactionContextDefaultState = {
  activeTransactionId: -1,
  setActiveTransactionId: () => {},
} as TransactionContextInterface;

const LocalizedStringsDefaultState = {
  localizedStrings: {
    languageId: "",
    strings: {},
  },
  setLocalizedStrings: () => {},
} as LocalizedStringsContextInterface;

export const TransactionContext = createContext(TransactionContextDefaultState);
export const PosClientIdContext = createContext(PosClientIdContextDefaultState);
export const LocalizedStringsContext = createContext(
  LocalizedStringsDefaultState,
);
