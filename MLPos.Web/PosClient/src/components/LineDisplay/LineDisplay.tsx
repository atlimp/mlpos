import "./LineDisplay.css";

import { useContext } from "react";
import { LocalizedStringsContext } from "../../context";

function LineDisplay({ lines, onDeleteLine }: LineDisplayProps) {
  const { localizedStrings } = useContext(LocalizedStringsContext);

  return (
    <div className="tableContainer">
      <table className="lineDisplay">
        <thead>
          <tr className="itemLineHeader">
            <th></th>
            <th className="itemLineName">
              {localizedStrings.strings["Description"]}
            </th>
            <th className="itemLinePrice">
              {localizedStrings.strings["Price"]}
            </th>
            <th className="itemLineQuantity">
              {localizedStrings.strings["Quantity"]}
            </th>
            <th className="itemLineAmount">
              {localizedStrings.strings["Amount"]}
            </th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {lines.map((line) => {
            return (
              <tr className="itemLine" key={line.id}>
                <td className="itemLineImageContainer">
                  <img className="itemLineImage" src={line.product.image}></img>
                </td>
                <td className="itemLineName">{line.product.name}</td>
                <td className="itemLinePrice">{line.product.price} kr.</td>
                <td className="itemLineQuantity">{line.quantity}</td>
                <td className="itemLineAmount">{line.amount} kr.</td>
                <td className="itemLineDelete">
                  <button
                    className="deleteButton"
                    onClick={() => {
                      onDeleteLine(line.id);
                    }}
                  >
                    🗑️
                  </button>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}

export default LineDisplay;
