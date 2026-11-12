<template>
  <div class="store">
    <header class="navbar">
      <h1 class="logo">ShopEase<span>.</span></h1>

      <nav>
        <a href="#home">Home</a>
        <a href="#products">Products</a>
        <a href="#about">About</a>
      </nav>

      <button class="cart-button" @click="showCart = true">
        🛒 Cart ({{ cartCount }})
      </button>  
    </header>

    <main>

      <section v-if="showCheckout && !orderPlaced" class="checkout-section">
        <button class="close-cart" @click="showCheckout = false">
          ← Back to Shop
        </button>

        <h2>Checkout</h2>
        <p>Complete your details to place your order.</p>

        <div class="checkout-layout">
          <form class="customer-form" @submit.prevent="placeOrder">
            <h3>Customer Details</h3>

            <label for="customer-name">Full Name</label>
            <input id="customer-name"
                   v-model.trim="customer.name"
                   type="text"
                   required
                   placeholder="Enter your full name" />

            <label for="customer-email">Email Address</label>
            <input id="customer-email"
                   v-model.trim="customer.email"
                   type="email"
                   required
                   placeholder="you@example.com" />

            <label for="customer-phone">Phone Number</label>
            <input id="customer-phone"
                   v-model.trim="customer.phone"
                   type="tel"
                   required
                   pattern="[0-9+ ()-]{7,20}"
                   placeholder="Enter your phone number" />

            <label for="customer-address">Delivery Address</label>
            <textarea id="customer-address"
                      v-model.trim="customer.address"
                      required
                      minlength="5"
                      placeholder="Enter your delivery address"
                      rows="4"></textarea>

            <button class="place-order-button" type="submit">
              Place Order
            </button>
          </form>

          <div class="order-summary">
            <h3>Order Summary</h3>

            <p v-if="cartItems.length === 0">
              Your cart is empty.
            </p>

            <div v-for="item in cartItems" :key="item.id" class="summary-item">
              <span>{{ item.name }} × {{ item.quantity }}</span>
              <strong>
                Rs. {{ (Number(item.price) * item.quantity).toLocaleString() }}
              </strong>
            </div>

            <div class="summary-total">
              <span>Total</span>
              <strong>Rs. {{ cartTotal.toLocaleString() }}</strong>
            </div>
          </div>
        </div>
      </section>

      <section v-if="orderPlaced" class="order-success">
        <h2>🎉 Order Placed Successfully!</h2>
        <p>Thank you, {{ customer.name || 'Customer' }}!</p>
        <p>Your demo order number is: <strong>{{ orderNumber }}</strong></p>
        <p>This is a demo confirmation. No payment was taken.</p>
        <button class="place-order-button" @click="orderPlaced = false">
          Continue Shopping
        </button>
      </section>

      <section v-if="showCart" class="cart-section">
        <div class="cart-heading">
          <h2>🛒 Your Shopping Cart</h2>
          <button class="close-cart" @click="showCart = false">
            Continue Shopping ✕
          </button>
        </div>

        <p v-if="cartItems.length === 0">
          Your cart is empty. Add some products!
        </p>

        <div v-else>
          <article v-for="item in cartItems"
                   :key="item.id"
                   class="cart-item">
            <div class="cart-item-info">
              <h3>{{ item.name }}</h3>
              <p>Rs. {{ Number(item.price).toLocaleString() }} each</p>
            </div>

            <div class="quantity-controls">
              <button @click="decreaseQuantity(item)">−</button>
              <span>{{ item.quantity }}</span>
              <button @click="increaseQuantity(item)">+</button>
            </div>

            <strong>
              Rs. {{ (Number(item.price) * item.quantity).toLocaleString() }}
            </strong>

            <button class="remove-button" @click="removeFromCart(item.id)">
              Remove
            </button>
          </article>

          <div class="cart-total">
            <h3>Total: Rs. {{ cartTotal.toLocaleString() }}</h3>

            <button class="place-order-button"
                    @click="goToCheckout">
              Proceed to Checkout →
            </button>
          </div>
        </div>
      </section>

      <section id="home" class="hero">
        <div class="hero-content">
          <p class="eyebrow">WELCOME TO SHOPEASE</p>
          <h2>Find Your<br />Everyday Essentials.</h2>
          <p>
            Discover quality products at great prices.
            Your next favourite item is waiting for you.
          </p>
          <a href="#products" class="shop-button">Shop Now →</a>
        </div>

        <div class="hero-art">
          <span class="hero-emoji">🛍️</span>
          <span class="floating-item">🎧</span>
          <span class="floating-item second">⌚</span>
        </div>
      </section>

      <section id="products" class="products-section">
        <div class="section-heading">
          <div>
            <p class="eyebrow">OUR COLLECTION</p>
            <h2>Featured Products</h2>
          </div>
          <p>Great finds, just for you.</p>
        </div>

        <div class="product-grid">
          <p v-if="loading">Loading products...</p>

          <p v-else-if="errorMessage">
            {{ errorMessage }}
          </p>
          
          <p v-else-if="products.length === 0">
            No products available.
          </p>

          <article class="product-card"
                   v-for="product in products"
                   :key="product.id">
            <div class="product-image">🛍️</div>

            <div class="product-info">
              <p class="category">Product</p>

              <h3>{{ product.name }}</h3>

              <p>{{ product.description }}</p>

              <p>Stock: {{ product.stockQuantity }}</p>

              <div class="product-bottom">
                <strong>
                  Rs. {{ Number(product.price).toLocaleString() }}
                </strong>

                <button @click="addToCart(product)"
                        class="add-button">
                  + Add
                </button>
              </div>
            </div>
          </article>
        </div>
      </section>

      <section id="about" class="about-section">
        <h2>Shopping made simple.</h2>
        <p>Thank you for visiting ShopEase. More exciting features are coming soon!</p>
      </section>
    </main>

    <footer>© 2026 ShopEase. Built with Vue.js.</footer>

  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'

const cartCount = computed(() =>
    cartItems.value.reduce((total, item) => total + item.quantity, 0)
)

const products = ref([])
const loading = ref(true)
const errorMessage = ref('')

const cartItems = ref([])
const showCart = ref(false)

const showCheckout = ref(false)
const orderPlaced = ref(false)
const orderNumber = ref('')

const customer = ref({
  name: '',
  email: '',
  phone: '',
  address: ''
})

function goToCheckout() {
  if (cartItems.value.length === 0) {
    alert('Please add products to your cart first.')
    return
  }

  showCart.value = false
  showCheckout.value = true
  orderPlaced.value = false
}

async function placeOrder() {
  if (cartItems.value.length === 0) {
    alert('Your cart is empty.')
    return
  }

  try {
    const orderData = {
      customerName: customer.value.name,
      email: customer.value.email,
      phone: customer.value.phone,
      deliveryAddress: customer.value.address,
      items: cartItems.value.map(item => ({
        productId: item.id,
        productName: item.name,
        quantity: item.quantity,
        unitPrice: Number(item.price)
      }))
    }

    const response = await fetch('http://localhost:5008/api/Orders', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(orderData)
    })

    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || 'Failed to place order.')
    }

    const savedOrder = await response.json()

    orderNumber.value = savedOrder.id
      ? 'SE' + String(savedOrder.id).padStart(8, '0')
      : 'SE' + Date.now().toString().slice(-8)

    orderPlaced.value = true
    showCheckout.value = false
    cartItems.value = []

    customer.value = {
      name: '',
      email: '',
      phone: '',
      address: ''
    }

  } catch (error) {
    console.error('Order error:', error)

    alert(
      error.message ||
      'Order එක save කරන්න බැරි වුණා. නැවත උත්සාහ කරන්න.'
    )
  }
}


const API_URL = 'http://localhost:5008/api/Products'

async function fetchProducts() {
    try {
      loading.value = true
      errorMessage.value = ''

      const response = await fetch(API_URL)

      if (!response.ok) {
        throw new Error('Failed to load products.')
      }

      products.value = await response.json()
    } catch (error) {
      errorMessage.value =
        'Products load කරන්න බැරි වුණා. Backend API එක run වෙනවාද බලන්න.'
      console.error(error)
    } finally {
      loading.value = false
    }
}

function addToCart(product) {
    const existingItem = cartItems.value.find(
      item => item.id === product.id
    )

    if (existingItem) {
      if (existingItem.quantity < product.stockQuantity) {
        existingItem.quantity++
      } else {
        alert('මෙම product එකේ තවත් stock නැහැ.')
      }
    } else {
      if (product.stockQuantity > 0) {
        cartItems.value.push({
          ...product,
          quantity: 1
        })
      } else {
        alert('මෙම product එක දැනට stock නැහැ.')
      }
    }

    showCart.value = false
}

function increaseQuantity(item) {
    if (item.quantity < item.stockQuantity) {
      item.quantity++
    } else {
      alert('Stock limit එකට ළඟා වී ඇත.')
    }
}

function decreaseQuantity(item) {
    if (item.quantity > 1) {
      item.quantity--
    } else {
      removeFromCart(item.id)
    }
}

function removeFromCart(productId) {
    cartItems.value = cartItems.value.filter(
      item => item.id !== productId
    )
}

const cartTotal = computed(() =>
    cartItems.value.reduce(
      (total, item) => total + Number(item.price) * item.quantity,
      0
    )
)

onMounted(() => {
    fetchProducts()
})
</script>

<style>
  * {
    box-sizing: border-box;
  }

  html {
    scroll-behavior: smooth;
  }

  body {
    margin: 0;
    font-family: Arial, Helvetica, sans-serif;
    color: #202433;
    background: #ffffff;
  }

  button,
  a {
    font: inherit;
  }

  .navbar {
    min-height: 76px;
    padding: 18px 7%;
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 24px;
    border-bottom: 1px solid #eeeeF2;
  }

  .logo {
    margin: 0;
    font-size: 27px;
    font-weight: 800;
  }

    .logo span {
      color: #e76f51;
    }

  nav {
    display: flex;
    gap: 28px;
  }

    nav a {
      color: #555b6e;
      text-decoration: none;
      font-size: 14px;
    }

      nav a:hover {
        color: #e76f51;
      }

  .cart-button,
  .add-button {
    border: 0;
    border-radius: 9px;
    padding: 11px 16px;
    cursor: pointer;
    font-weight: 700;
  }

  .cart-button {
    background: #202433;
    color: white;
  }

  .hero {
    margin: 28px 5%;
    padding: 50px 7%;
    border-radius: 24px;
    background: #fff2eb;
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 30px;
    min-height: 350px;
  }

  .hero-content {
    max-width: 520px;
  }

  .eyebrow {
    color: #d65e43;
    font-size: 12px;
    font-weight: 800;
    letter-spacing: 2px;
  }

  .hero h2 {
    margin: 15px 0;
    font-size: clamp(34px, 5vw, 54px);
    line-height: 1.12;
  }

  .hero-content > p:not(.eyebrow) {
    color: #6c7080;
    line-height: 1.8;
    max-width: 430px;
  }

  .shop-button {
    display: inline-block;
    margin-top: 15px;
    padding: 14px 22px;
    border-radius: 9px;
    background: #e76f51;
    color: white;
    text-decoration: none;
    font-weight: 700;
  }

  .hero-art {
    position: relative;
    width: 230px;
    height: 220px;
    display: grid;
    place-items: center;
    flex-shrink: 0;
    border-radius: 50%;
    background: #f9d4c3;
  }

  .hero-emoji {
    font-size: 100px;
  }

  .floating-item {
    position: absolute;
    top: 10px;
    right: -4px;
    font-size: 38px;
    padding: 12px;
    border-radius: 16px;
    background: white;
    box-shadow: 0 8px 25px #00000012;
  }

    .floating-item.second {
      top: auto;
      right: auto;
      left: -12px;
      bottom: 12px;
    }

  .products-section {
    padding: 35px 7% 60px;
  }

  .section-heading {
    display: flex;
    justify-content: space-between;
    align-items: end;
    gap: 20px;
    margin-bottom: 28px;
  }

    .section-heading h2 {
      margin: 8px 0 0;
      font-size: 30px;
    }

    .section-heading > p {
      color: #777c8b;
    }

  .product-grid {
    display: grid;
    grid-template-columns: repeat(4, minmax(0, 1fr));
    gap: 22px;
  }

  .product-card {
    overflow: hidden;
    border: 1px solid #eeeeF2;
    border-radius: 15px;
    transition: transform 0.2s, box-shadow 0.2s;
  }

    .product-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 12px 30px #0000000c;
    }

  .product-image {
    height: 165px;
    display: grid;
    place-items: center;
    background: #f6f6f9;
    font-size: 65px;
  }

  .product-info {
    padding: 16px;
  }

  .category {
    color: #8a8e9c;
    font-size: 12px;
  }

  .product-info h3 {
    font-size: 16px;
    min-height: 38px;
  }

  .product-bottom {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 8px;
  }

    .product-bottom strong {
      font-size: 14px;
    }

  .add-button {
    background: #fff0e9;
    color: #d65e43;
    padding: 9px 12px;
  }

  .about-section {
    padding: 35px 7%;
    text-align: center;
    background: #f7f7fa;
  }

    .about-section p {
      color: #777c8b;
    }

  footer {
    padding: 24px;
    text-align: center;
    color: #777c8b;
    font-size: 13px;
  }

  @media (max-width: 850px) {
    .product-grid {
      grid-template-columns: repeat(2, minmax(0, 1fr));
    }

    .hero {
      padding: 35px;
    }

    .hero-art {
      width: 170px;
      height: 170px;
    }

    .hero-emoji {
      font-size: 75px;
    }
  }

  @media (max-width: 560px) {
    .navbar {
      padding: 16px 5%;
      flex-wrap: wrap;
    }

    nav {
      order: 3;
      width: 100%;
      justify-content: center;
    }

    .hero {
      margin: 18px 4%;
      padding: 28px 22px;
    }

    .hero-art {
      display: none;
    }

    .section-heading {
      display: block;
    }

    .product-grid {
      gap: 12px;
    }

    .product-image {
      height: 120px;
    }

    .product-info {
      padding: 11px;
    }

      .product-info h3 {
        font-size: 14px;
      }

    .product-bottom {
      align-items: start;
      flex-direction: column;
    }
  }

  .cart-section {
    margin: 25px 7%;
    padding: 25px;
    border: 1px solid #eeeeF2;
    border-radius: 15px;
    background: #ffffff;
  }

  .cart-heading {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 12px;
    flex-wrap: wrap;
    margin-bottom: 20px;
  }

    .cart-heading h2 {
      margin: 0;
    }

  .close-cart,
  .quantity-controls button,
  .remove-button {
    padding: 9px 12px;
    border: 0;
    border-radius: 7px;
    cursor: pointer;
  }

  .close-cart {
    background: #202433;
    color: white;
  }

  .cart-item {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 15px;
    flex-wrap: wrap;
    padding: 18px 0;
    border-bottom: 1px solid #eeeeF2;
  }

  .cart-item-info h3,
  .cart-item-info p {
    margin: 5px 0;
  }

  .cart-item-info p {
    color: #777c8b;
  }

  .quantity-controls {
    display: flex;
    align-items: center;
    gap: 12px;
  }

    .quantity-controls button {
      background: #fff0e9;
      color: #d65e43;
      font-weight: bold;
    }

  .remove-button {
    background: #ffe8e8;
    color: #c62828;
  }

  .cart-total {
    margin-top: 20px;
    text-align: right;
  }

  @media (max-width: 560px) {
    .cart-section {
      margin: 18px 4%;
      padding: 16px;
    }

    .cart-item {
      align-items: flex-start;
      flex-direction: column;
    }
  }

  .checkout-section {
    margin: 30px 7%;
    padding: 28px;
    border: 1px solid #eeeeF2;
    border-radius: 16px;
    background: #fff;
  }

    .checkout-section > h2 {
      margin-bottom: 8px;
    }

  .checkout-layout {
    display: grid;
    grid-template-columns: 1.3fr 1fr;
    gap: 28px;
    margin-top: 25px;
  }

  .customer-form,
  .order-summary {
    padding: 22px;
    border: 1px solid #eeeeF2;
    border-radius: 12px;
  }

  .customer-form {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }

    .customer-form h3,
    .order-summary h3 {
      margin-top: 0;
    }

    .customer-form label {
      font-weight: 600;
      font-size: 14px;
    }

    .customer-form input,
    .customer-form textarea {
      width: 100%;
      padding: 12px;
      border: 1px solid #d8dbe3;
      border-radius: 8px;
      font: inherit;
    }

      .customer-form input:focus,
      .customer-form textarea:focus {
        outline: 2px solid #e76f51;
        border-color: transparent;
      }

  .place-order-button {
    margin-top: 15px;
    padding: 13px 18px;
    border: none;
    border-radius: 9px;
    background: #e76f51;
    color: white;
    font-weight: 700;
    cursor: pointer;
  }

    .place-order-button:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

  .summary-item,
  .summary-total {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    gap: 12px;
    padding: 14px 0;
    border-bottom: 1px solid #eeeeF2;
  }

    .summary-item strong {
      white-space: nowrap;
    }

  .summary-total {
    margin-top: 12px;
    font-size: 18px;
    border-bottom: none;
  }

  .order-success {
    margin: 30px 7%;
    padding: 35px 20px;
    border-radius: 16px;
    background: #f0faf2;
    text-align: center;
  }

    .order-success h2 {
      color: #247a3d;
    }

  @media (max-width: 700px) {
    .checkout-section {
      margin: 20px 4%;
      padding: 18px;
    }

    .checkout-layout {
      grid-template-columns: 1fr;
    }

    .customer-form,
    .order-summary {
      padding: 16px;
    }
  }
</style>
