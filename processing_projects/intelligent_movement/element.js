class Element {
  constructor() {
    this.position = createVector(random(width),random(height));
    this.velocity = createVector();
    this.d_velocity = createVector();
    this.top_speed = 5;
    this.v_width = 10;
    this.v_length= 40;
  }

  update() {
    var mouse = createVector(mouseX,mouseY);
    this.d_velocity = p5.Vector.sub(mouse, this.position);
    var d = this.d_velocity.mag();
    this.d_velocity.normalize();
    if (d < 20) {
      var dist = mouse.dist(this.position);
      console.log(dist);
      var lerp_val = lerp(dist, 0, 0.5);
    //  console.log(lerp_val);
      this.d_velocity.mult(lerp_val); 
    } else {
      this.d_velocity.mult(this.top_speed);
    }
    //this.d_velocity.mult(this.top_speed);
    //}
    
    var steering_force = p5.Vector.sub(this.d_velocity, this.velocity);
    steering_force.limit(0.8);
    this.velocity.add(steering_force);
    this.position.add(this.velocity);
  }
  
  checkEdges() {
    if (this.position.x > width) {
      this.position.x = width;
    }
    else if (this.position.x < 0) {
      this.position.x = 0;
    }

    if (this.position.y > height - this.v_length) {
      this.position.y = height - this.v_length;
    }
    else if (this.position.y < 0) {
      this.position.y = 0;
    }
  }

  display() {
    stroke(0);
    strokeWeight(2);
    fill(255);
    triangle(this.position.x - this.v_width,     // x1
             this.position.y,                    // y1
             this.position.x + this.v_width,     // x2
             this.position.y,                    // y2
             this.position.x,                    // x3
             this.position.y + this.v_length);   // y3
  }
}
