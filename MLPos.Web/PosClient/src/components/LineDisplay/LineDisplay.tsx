import './LineDisplay.css';

function LineDisplay({ lines, onDeleteLine }: LineDisplayProps) {
    return (
        <div className="tableContainer">
            <table className="lineDisplay">
                <thead>
                    <tr className="itemLineHeader">
                        <th></th>
                        <th className="itemLineName">Description</th>
                        <th className="itemLinePrice">Price</th>
                        <th className="itemLineQuantity">Quantity</th>
                        <th className="itemLineAmount">Amount</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                        {lines.map(line => {
                            return (
                                <tr className="itemLine" key={line.id}>
                                    <td className="itemLineImageContainer"><img className="itemLineImage" src={line.product.image}></img></td>
                                    <td className="itemLineName">{line.product.name}</td>
                                    <td className="itemLinePrice">{line.product.price} kr.</td>
                                    <td className="itemLineQuantity">{line.quantity}</td>
                                    <td className="itemLineAmount">{line.amount} kr.</td>
                                    <td className="itemLineDelete"><button
                                        className="deleteButton"
                                        onClick={() => { onDeleteLine(line.id) }}
                                    >
                                        🗑️</button></td>
                                </tr>
                            );
                        })}
                </tbody>
            </table>
        </div>
    );
}

export default LineDisplay;