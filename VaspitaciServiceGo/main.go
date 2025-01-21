package main

import (
	"fmt"
	"log"
	"net/http"

	"github.com/gin-gonic/gin"
	"gorm.io/driver/mysql"
	"gorm.io/gorm"
)

// Model za korisnike
type User struct {
	ID    uint   `gorm:"primaryKey" json:"id"`
	Name  string `json:"name"`
	Email string `json:"email"`
}

// Globalna promenljiva za DB konekciju
var DB *gorm.DB

func main() {
	// Detalji baze podataka
	dbUser := "root"
	dbPassword := "haris123"
	dbHost := "mysql"
	dbName := "vaspitac_db"

	// DSN za MySQL
	dsn := fmt.Sprintf("%s:%s@tcp(%s:3306)/%s?charset=utf8mb4&parseTime=True&loc=Local",
		dbUser, dbPassword, dbHost, dbName)

	// Povezivanje na bazu
	var err error
	DB, err = gorm.Open(mysql.Open(dsn), &gorm.Config{})
	if err != nil {
		log.Fatalf("Ne mogu da se povežem na MySQL: %v", err)
	}
	log.Println("Konekcija na MySQL uspešna.")

	// Automatsko kreiranje tabele (migracija)
	if err := DB.AutoMigrate(&User{}); err != nil {
		log.Fatalf("Greška pri migraciji: %v", err)
	}
	log.Println("Tabela `users` je migrirana ili već postoji.")

	// Gin router
	r := gin.Default()

	// Rute
	r.GET("/", helloRoute)
	r.GET("/users", getUsers)          // Dohvati sve korisnike
	r.GET("/users/:id", getUserByID)   // Dohvati korisnika po ID-u
	r.POST("/users", createUser)       // Dodaj novog korisnika
	r.PUT("/users/:id", updateUser)    // Ažuriraj korisnika
	r.DELETE("/users/:id", deleteUser) // Obriši korisnika

	// Pokreni server na portu 8080
	err = r.Run(":8080")
	if err != nil {
		log.Fatalf("Greška prilikom pokretanja servera: %v", err)
	}
}

// Root ruta
func helloRoute(c *gin.Context) {
	c.String(http.StatusOK, "Dobrodošli u VaspitacService! Koristite /users GET ili POST.")
}

// GET /users - Dohvata sve korisnike
func getUsers(c *gin.Context) {
	var users []User
	if err := DB.Find(&users).Error; err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusOK, users)
}

// GET /users/:id - Dohvata korisnika po ID-u
func getUserByID(c *gin.Context) {
	id := c.Param("id")
	var user User
	if err := DB.First(&user, id).Error; err != nil {
		c.JSON(http.StatusNotFound, gin.H{"error": "Korisnik nije pronađen"})
		return
	}
	c.JSON(http.StatusOK, user)
}

// POST /users - Dodaje novog korisnika
func createUser(c *gin.Context) {
	var user User
	if err := c.BindJSON(&user); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	if err := DB.Create(&user).Error; err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusCreated, user)
}

// PUT /users/:id - Ažurira korisnika po ID-u
func updateUser(c *gin.Context) {
	id := c.Param("id")
	var user User
	if err := DB.First(&user, id).Error; err != nil {
		c.JSON(http.StatusNotFound, gin.H{"error": "Korisnik nije pronađen"})
		return
	}

	var updatedUser User
	if err := c.BindJSON(&updatedUser); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	user.Name = updatedUser.Name
	user.Email = updatedUser.Email

	if err := DB.Save(&user).Error; err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusOK, user)
}

// DELETE /users/:id - Briše korisnika po ID-u
func deleteUser(c *gin.Context) {
	id := c.Param("id")
	var user User
	if err := DB.First(&user, id).Error; err != nil {
		c.JSON(http.StatusNotFound, gin.H{"error": "Korisnik nije pronađen"})
		return
	}

	if err := DB.Delete(&user).Error; err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusNoContent, nil)
}
