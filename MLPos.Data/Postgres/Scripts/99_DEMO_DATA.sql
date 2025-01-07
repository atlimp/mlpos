INSERT INTO CUSTOMER(name, email, image) VALUES('Demo Demosson', 'demo@demo.is', 'https://t3.ftcdn.net/jpg/02/22/85/16/360_F_222851624_jfoMGbJxwRi5AWGdPgXKSABMnzCQo9RN.jpg');
INSERT INTO CUSTOMER(name, email, image) VALUES('Demína Demodóttir', 'demina@demo.is', 'https://images.pexels.com/photos/774909/pexels-photo-774909.jpeg?auto=compress&cs=tinysrgb&dpr=1&w=500');
INSERT INTO CUSTOMER(name, email, image) VALUES('Jón Jónsson', 'jonjonsson@example.com', 'https://images.pexels.com/photos/2379004/pexels-photo-2379004.jpeg?auto=compress&cs=tinysrgb&dpr=1&w=500');
INSERT INTO CUSTOMER(name, email, image) VALUES('Gamli Gamlason', 'gamligamli@dv.is', 'https://image1.masterfile.com/getImage/NjAwLTAxOTU0MjcxZW4uMDAwMDAwMDA=AH26h9/600-01954271en_Masterfile.jpg');
INSERT INTO CUSTOMER(name, email, image) VALUES('Forstjóri Forstjórason', 'forstjóri@ble.is', 'https://media.istockphoto.com/id/1364917563/photo/businessman-smiling-with-arms-crossed-on-white-background.jpg?s=612x612&w=0&k=20&c=NtM9Wbs1DBiGaiowsxJY6wNCnLf0POa65rYEwnZymrM=');
INSERT INTO CUSTOMER(name, email, image) VALUES('Jónína Jóhannsdóttir', 'jonina@ble.is', 'https://cdn.create.vista.com/api/media/small/250363326/stock-photo-smiling-attractive-woman-white-sweater-looking-camera-isolated-pink');

INSERT INTO PRODUCT(name, type, price, image) VALUES('Coca cola', 0, 250, 'https://dtgxwmigmg3gc.cloudfront.net/imagery/assets/derivations/icon/512/512/true/eyJpZCI6ImUzODdkNmM1NmRmNWRlYzNiYmFiN2UzNTIyNGY0ZDM5Iiwic3RvcmFnZSI6InB1YmxpY19zdG9yZSJ9?signature=561844f30ebfd64c2a24c454cb2e8f734450a35af1577252ae03284a741a7f34');
INSERT INTO PRODUCT(name, type, price, image) VALUES('Kristall', 0, 250, 'https://dtgxwmigmg3gc.cloudfront.net/imagery/assets/derivations/icon/512/512/true/eyJpZCI6ImUzODdkNmM1NmRmNWRlYzNiYmFiN2UzNTIyNGY0ZDM5Iiwic3RvcmFnZSI6InB1YmxpY19zdG9yZSJ9?signature=561844f30ebfd64c2a24c454cb2e8f734450a35af1577252ae03284a741a7f34');
INSERT INTO PRODUCT(name, type, price, image) VALUES('Egils Gull', 0, 500, 'https://nammi.is/cdn/shop/products/egils-gull-5-33-cl-667386.jpg?v=1650211909');
INSERT INTO PRODUCT(name, type, price, image) VALUES('Tuborg Classic', 0, 500, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSg0CMN-L2eLeUBOpsntWXMkdeQ8icbwr0G4A&s');
INSERT INTO PRODUCT(name, type, price, image) VALUES('Egils Lite', 0, 500, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR5WQRgsg1SnSbClSPxMuHR9iA65VUZS7CsZA&s');
INSERT INTO PRODUCT(name, type, price, image) VALUES('Prins Polo', 0, 150, 'https://nyjavinbudin.is/wp-content/uploads/2023/11/prins-polo-600x600.png.jpg');
INSERT INTO PRODUCT(name, type, price, image) VALUES('Doritos', 0, 300, 'https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcR_FnLHMwZjI2gVgq8q8QDh2lf1KhKyfmtzcvBR9DStxqBF-l58XWpcVhMcwcB7Z6m0cqJbtaQ9OYjxlO3ByNZuP1RAzFFjPQTlGnDt-TQ');
INSERT INTO PRODUCT(name, type, price, image) VALUES('Klukkutími í golf hermi', 1, 2000, 'https://vikurgolf.simplybook.it/uploads/vikurgolf/image_files/preview/a7d4eea45033140361a0f81f91d973d9.jpg');
INSERT INTO PRODUCT(name, type, price, image) VALUES('10 tíma kort', 1, 18000, 'https://vikurgolf.simplybook.it/uploads/vikurgolf/image_files/preview/a7d4eea45033140361a0f81f91d973d9.jpg');
INSERT INTO PRODUCT(name, type, price, image) VALUES('Mótsgjald', 1, 3500, 'https://vikurgolf.simplybook.it/uploads/vikurgolf/image_files/preview/a7d4eea45033140361a0f81f91d973d9.jpg');

INSERT INTO PAYMENTMETHOD(name, description, image) VALUES('Í Reikning', 'Upphæð {amount} verður skráð sem úttekt.  Gert er upp í lok mánaðar.', 'https://media.istockphoto.com/id/495477978/photo/open-book.jpg?s=612x612&w=0&k=20&c=vwJ6__M7CVPdjkQFUv9j2pr7QJiQ9bWW_5jXjR9TcjY=');
INSERT INTO PAYMENTMETHOD(name, description, image) VALUES('Millifært', 'Vinsamlega legðu inn {amount} á reikning 0111-26-0000001', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQxJh203_wQQ4n2Mw8y8y15xviF0qG80AjFJA&s');
INSERT INTO PAYMENTMETHOD(name, description, image) VALUES('Aur', 'Vinsamlega borgaðu {amount} með Aur með símanúmer 58-12345', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSKszQVt1ulGzdMZ9EzaWWI0j2BBFMbKzmQFg&s');

INSERT INTO POSCLIENT(name, description) VALUES('Aðal client', '');