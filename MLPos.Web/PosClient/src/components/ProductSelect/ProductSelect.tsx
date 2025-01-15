import "./ProductSelect.css";
import { useEffect, useState } from "react";
import Api from "../../api/api";
import leftArrow from "../../assets/icons/left-arrow.png";

function ProductSelect({ onSelectProduct }: ProductSelectProps) {
  const [products, setProducts] = useState<Product[]>([]);

  const getAllProducts = async () => {
    const api = new Api({ posClientId: -1 });
    const products = await api.getAllProducts();
    setProducts(products);
  };

  useEffect(() => {
    getAllProducts();
  }, []);

  return (
    <div className="modal">
      <div className="modalContent productSelection">
        <img
          className="back"
          src={leftArrow}
          onClick={() => {
            onSelectProduct(-1);
          }}
        />
        <div className="productList">
          {products.map((product: Product) => {
            return (
              <div
                className="productCard"
                key={product.id}
                onClick={() => onSelectProduct(product.id)}
              >
                <div>
                  <div className="productImageContainer">
                    <img src={product.image}></img>
                  </div>
                </div>
                <div className="productInfoContainer">
                  <div className="productName">{product.name}</div>
                  <div className="productPrice">{product.price} kr.</div>
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}

export default ProductSelect;
