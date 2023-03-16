<%@ page contentType="text/html;charset=UTF-8" language="java"%>
<%@taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@taglib prefix="form" uri="http://www.springframework.org/tags/form" %>
<%@taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"  %>
<html lang="en" xmlns:th="http://www.thymeleaf.org">
<head>
<title>OCR Metrics</title>
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
<link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/1.10.25/css/jquery.dataTables.min.css">

<script type="text/javascript" src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script type="text/javascript" src="https://cdn.datatables.net/1.10.25/js/jquery.dataTables.min.js"></script>
<script type="text/javascript">
	$(document).ready(function() {
		$("#report").DataTable({
			"order" : [ [ 1, "asc" ], [ 0, "asc" ] ],
			"pageLength" : 10,
			"lengthMenu" : [ 5, 10, 50, 100 ]
		});
	});
</script>
</head>
<body>
	<h1>OCR Metrics</h1>

	<nav aria-label="breadcrumb">
		<ol class="breadcrumb">
			<li class="breadcrumb-item"><a href="/report">Dashboard</a></li>
		</ol>
	</nav>

	<div class="panel-body">
		<form:form method="post" modelAttribute="cmd">
			<button type="submit" class="btn btn-primary" name="save">Get Report</button>
			<br />
			<br />
			<table class="table" style="width: auto;">
				<tbody>
					<tr>
						<td>Total number of mismatches</td>
						<td>${mismatchCount}</td>
					</tr>
					<tr>
						<td>Total number of fields</td>
						<td>${totalCount}</td>
					</tr>
					<tr>
						<td>Success rate</td>
						<td><fmt:formatNumber value="${successRate}" maxFractionDigits="3" /></td>
					</tr>
				</tbody>
			</table>
			<table id="report" class="display">
				<thead>
					<tr>
						<td>File Name</td>
						<td>Field Name</td>
						<td>Expected Value</td>
						<td>Actual Value</td>
						<td>Confidence</td>
					</tr>
				</thead>
				<tbody>
					<c:forEach var="row" items="${fieldComparisons}">
						<tr>
							<td>${row.fileName}</td>
							<td>${row.fieldName}</td>
							<td>${row.expected}</td>
							<td style="white-space: pre;">${row.actual}</td>
							<td>${row.confidence}</td>
						</tr>
					</c:forEach>
				</tbody>
			</table>
		</form:form>
	</div>
</body>
</html>