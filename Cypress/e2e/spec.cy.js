// cypress/e2e/menu_buttons_spec.cy.js

describe('SolomikovPod Online Store - Menu Button Functionality', () => {
  
  beforeEach(() => {
    // Visit the home page before each test
    cy.visit('https://localhost:7151');
  });

  it('Should navigate to Home page when clicking Home button', () => {
    // Click on Home button in the menu
    cy.contains('Home').click();

    // Verify that the URL includes the home route and the correct text is displayed
    cy.url().should('include', '/');
    cy.contains('Welcome to SolomikovPod Online Store').should('be.visible');
  });

  it('Should navigate to Products page when clicking Products button', () => {
    // Click on Products button in the menu
    cy.contains('Products').click();

    // Verify that the URL includes the products route and the product list is displayed
    cy.url().should('include', '/products');
    cy.contains('Products').should('be.visible');
    cy.get('.product-card').should('have.length.greaterThan', 0);
  });

  it('Should navigate to Register page when clicking Register button', () => {
    // Click on Register button in the menu
    cy.contains('Register').click();

    // Verify that the URL includes the register route and the registration form is displayed
    cy.url().should('include', '/register');
    cy.contains('Register').should('be.visible');
    cy.get('input').should('have.length', 3); // Username, Password, Email fields
  });

  it('Should navigate to Login page when clicking Login button', () => {
    // Click on Login button in the menu
    cy.contains('Login').click();

    // Verify that the URL includes the login route and the login form is displayed
    cy.url().should('include', '/login');
    cy.contains('Login').should('be.visible');
    cy.get('input').should('have.length', 3); // Username, Password, Email fields
  });

  it('Should navigate to Cart page after login when clicking Cart button', () => {
    // First navigate to the login page and log in
    cy.contains('Login').click();
    cy.get('input').eq(0).type('TestUser'); // Input Username
    cy.get('input').eq(1).type('TestPassword123'); // Input Password
    cy.get('input').eq(2).type('testuser@example.com'); // Input Email
    cy.contains('Login').click(); // Click Login button

    // Wait to ensure login process is completed
    cy.wait(1000);

    // Click on "Products" without refreshing the page
    cy.contains('Products').click(); 

    // Wait to ensure products are being loaded
    cy.wait(2000); 

    // Verify that products are displayed after clicking "Products"
    cy.get('.product-card').should('have.length.greaterThan', 0);
    });

    it('Should logout user and redirect to login page when clicking Logout button', () => {
      // First navigate to the login page and log in
      cy.contains('Login').click();
      cy.get('input').eq(0).type('TestUser');
      cy.get('input').eq(1).type('TestPassword123');
      cy.get('input').eq(2).type('testuser@example.com');
      cy.contains('Login').click();
      cy.wait(2000); 
  
      cy.contains('Products').click(); 
      cy.wait(1000);
      
    });
});
