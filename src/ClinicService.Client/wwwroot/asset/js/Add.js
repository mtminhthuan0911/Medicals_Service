
						// Get the modal
							var modal1 = document.getElementById("myModal1");

							// Get the button that opens the modal
							var btn = document.getElementById("myBtn1");

							// Get the <span> element that closes the modal
							var span = document.getElementsByClassName("close")[0];

							// When the user clicks on the button, open the modal
							btn.onclick = function() {
							modal1.style.display = "block";
							}

							// When the user clicks on <span> (x), close the modal
							span.onclick = function() {
							modal1.style.display = "none";
							}

							// When the user clicks anywhere outside of the modal, close it
							window.onclick = function(event) {
							if (event.target == modal1) {
								modal1.style.display = "none";
							}
              }
              
//scroll button

//Nhận button
var mybutton = document.getElementById("myBtn");

// Khi người dùng cuộn được 10line , button Top sẽ Xuất hiện
window.onscroll = function () { scrollFunction() };

function topFunction() {
	$("html, body").animate({ scrollTop: 0 }, 1000);
}

function scrollFunction() {
	if (document.body.scrollTop > 5000 || document.documentElement.scrollTop > 0) {
		mybutton.style.display = "block";
	} else {
		mybutton.style.display = "none";
	}
}

// function - xử lí khi click vào button
 $(document).ready(function () {
	// Add smooth scrolling to all links
	$('b').on('click', function (event) {

		// Make sure this.hash has a value before overriding default behavior
		if (this.hash !== "") {
			// Prevent default anchor click behavior
			event.preventDefault();

			// Store hash
			var hash = this.hash;

			// Using jQuery's animate() method to add smooth page scroll
			// The optional number (800) specifies the number of milliseconds it takes to scroll to the specified area
			$('html, body').animate({
				scrollTop: $(hash).offset().top
			}, 800, function () {

				// Add hash (#) to URL when done scrolling (default click behavior)
				window.location.hash = hash;
			});
		} // End if
	});
});

// popup message 
//function openForm() {
//  document.getElementById("myForm").style.display = "block";
//}

//function closeForm() {
//  document.getElementById("myForm").style.display = "none";
//}
//function myFunction()
//{
//	confirm("Bạn Cần Phải Đăng Nhập")
//}
// Test Input by input.oninvalid = function(event)
//var input = document.getElementById('fname', 'firstname','lastname','phone');
//// khai báo Biến input
//	input.onivalid = function (envent)
//	{
//		envent.target.setCustomValidity('Bạn nên Nhập Vào Định Dạng Hợp Lệ .e.g ABC');
//			}