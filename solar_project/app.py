"""Main Flask application for managing solar panel efficiency data."""

from datetime import datetime
import json

from flask import Flask, render_template, request, redirect, url_for, flash
from sqlalchemy.exc import SQLAlchemyError

from models import db, SolarData


# Initialize Flask app
app = Flask(__name__)
app.config["SECRET_KEY"] = "dev-secret-key-change-this"
app.config["SQLALCHEMY_DATABASE_URI"] = "sqlite:///database.db"
app.config["SQLALCHEMY_TRACK_MODIFICATIONS"] = False

# Initialize SQLAlchemy with app
db.init_app(app)


# Create database tables automatically on first run
with app.app_context():
    db.create_all()


def parse_positive_float(value: str, field_name: str) -> float:
    """
    Validate and convert a form value into a positive float.

    Args:
        value: Raw string value from form input.
        field_name: Human-friendly field name for error messages.

    Returns:
        Positive float value.

    Raises:
        ValueError: If value is empty, non-numeric, or not positive.
    """
    if value is None or value.strip() == "":
        raise ValueError(f"{field_name} is required.")

    try:
        numeric_value = float(value)
    except ValueError as exc:
        raise ValueError(f"{field_name} must be a numeric value.") from exc

    if numeric_value <= 0:
        raise ValueError(f"{field_name} must be greater than 0.")

    return numeric_value


@app.route("/")
def dashboard():
    """
    Dashboard route.

    Shows:
    - Total records
    - Average efficiency
    - Latest reading
    - Efficiency-over-time chart data
    """
    total_records = SolarData.query.count()
    avg_efficiency = db.session.query(db.func.avg(SolarData.efficiency)).scalar()
    latest_reading = SolarData.query.order_by(SolarData.timestamp.desc()).first()

    readings = SolarData.query.order_by(SolarData.timestamp.asc()).all()
    chart_labels = [reading.timestamp.strftime("%Y-%m-%d %H:%M:%S") for reading in readings]
    chart_values = [reading.efficiency for reading in readings]

    return render_template(
        "index.html",
        total_records=total_records,
        avg_efficiency=round(avg_efficiency, 2) if avg_efficiency is not None else None,
        latest_reading=latest_reading,
        chart_labels=json.dumps(chart_labels),
        chart_values=json.dumps(chart_values),
    )


@app.route("/add", methods=["GET", "POST"])
def add_data():
    """
    Add new solar data reading.

    GET: Render add form.
    POST: Validate input, calculate power, store record, and redirect.
    """
    if request.method == "POST":
        voltage_input = request.form.get("voltage", "")
        current_input = request.form.get("current", "")
        efficiency_input = request.form.get("efficiency", "")

        try:
            voltage = parse_positive_float(voltage_input, "Voltage")
            current = parse_positive_float(current_input, "Current")
            efficiency = parse_positive_float(efficiency_input, "Efficiency")

            # Power calculation logic: P = V * I
            power = voltage * current

            solar_entry = SolarData(
                voltage=voltage,
                current=current,
                power=power,
                efficiency=efficiency,
                timestamp=datetime.utcnow(),
            )

            db.session.add(solar_entry)
            db.session.commit()
            flash("Solar reading added successfully.", "success")
            return redirect(url_for("data_table"))

        except ValueError as validation_error:
            flash(str(validation_error), "danger")

        except SQLAlchemyError:
            db.session.rollback()
            flash("Database error occurred while saving data.", "danger")

    return render_template("add.html")


@app.route("/data")
def data_table():
    """Display all solar records in a table sorted by newest first."""
    records = SolarData.query.order_by(SolarData.timestamp.desc()).all()
    return render_template("data.html", records=records)


@app.route("/delete/<int:record_id>", methods=["POST"])
def delete_data(record_id: int):
    """Delete a specific record safely by ID."""
    record = SolarData.query.get_or_404(record_id)

    try:
        db.session.delete(record)
        db.session.commit()
        flash(f"Record #{record_id} deleted successfully.", "success")
    except SQLAlchemyError:
        db.session.rollback()
        flash("Could not delete record due to a database error.", "danger")

    return redirect(url_for("data_table"))


if __name__ == "__main__":
    app.run(debug=True)
